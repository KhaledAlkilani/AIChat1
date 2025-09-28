using AIChat1.Entity;
using AIChat1.Entity.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using server.Hubs;
using Swashbuckle.AspNetCore.Annotations;
using AIChat1.IService;

namespace AIChat1.Controller
{
    public record SendMessageRequest(int UserId, string Content);
    public record MessageDto(int Id, int UserId, string Content, DateTime SentAt, string Username);

    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly AppDbContext _db;
        private readonly IChatService _chatSvc;
        public record StartConversationRequest(int UserId);

        public ChatController(IHubContext<ChatHub> chatHub, AppDbContext db, IChatService chatSvc)
        {
            _chatHub = chatHub;
            _db = db;
            _chatSvc = chatSvc;
        }

        // POST /api/chat/send
        [HttpPost("send")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
        [SwaggerOperation(OperationId = "SendMessage")] // this becomes the generated client method name
        public async Task<ActionResult<MessageDto>> Send([FromBody] SendMessageRequest req)
        {
            // 1) resolve user
            var user = await _db.Users.FindAsync(req.UserId);
            if (user is null) return NotFound($"User {req.UserId} not found.");

            // 2) get or create an active conversation for this user
            var conv = await _db.Conversations
                .FirstOrDefaultAsync(c => c.UserId == req.UserId && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow));

            if (conv is null)
            {
                conv = new Conversation { UserId = req.UserId, CreatedAt = DateTime.UtcNow };
                _db.Conversations.Add(conv);
                await _db.SaveChangesAsync(); // ensure conv.Id is available
            }

            // 3) persist message with required FK
            var entity = new Message
            {
                ConversationId = conv.Id,
                UserId = req.UserId,
                Content = req.Content,
                SentAt = DateTime.UtcNow,
                Sender = MessageSender.User
            };
            _db.Messages.Add(entity);
            await _db.SaveChangesAsync();

            // 4) DTO + broadcast
            var dto = new MessageDto(entity.Id, entity.UserId, entity.Content, entity.SentAt, user.Username);
            await _chatHub.Clients.User(user.Id.ToString()).SendAsync("ReceiveMessage", user.Username, dto.Content);
             
            // 5) Build short conversation history and ask the LLM
            string? aiReply = null;
            try
            {
                var rows = await _db.Messages
                    .Where(m => m.ConversationId == conv.Id)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();

                // keep last N turns to control token usage
                const int maxTurns = 20;
                var recent = rows.Count > maxTurns ? rows.Skip(rows.Count - maxTurns) : rows;

                // convert to LLM messages
                var llmMsgs = new List<LlmMsg>
    {
        new("system", "You are an assistant in a desktop AI chat app. Be concise and helpful.")
    };

                foreach (var m in recent)
                {
                    var role = m.Sender == MessageSender.User ? "user" : "assistant";
                    var content = m.Sender == MessageSender.User ? $"{user.Username}: {m.Content}" : m.Content;
                    llmMsgs.Add(new LlmMsg(role, content));
                }

                aiReply = await _chatSvc.GetAiResponseWithHistoryAsync(llmMsgs, HttpContext.RequestAborted);
            }
            catch
            {
                aiReply = null;
            }

            // 6) Persist and broadcast the AI reply
            if (!string.IsNullOrWhiteSpace(aiReply))
            {
                var aiMsg = new Message
                {
                    ConversationId = conv.Id,
                    UserId = req.UserId,
                    Content = aiReply!,
                    SentAt = DateTime.UtcNow,
                    Sender = MessageSender.Assistant,
                };
                _db.Messages.Add(aiMsg);
                await _db.SaveChangesAsync();

                await _chatHub.Clients.User(user.Id.ToString())
                    .SendAsync("ReceiveMessage", "AI", aiReply);
            } 

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }
        
        // POST /api/chat/start
        [HttpPost("start")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
        [SwaggerOperation(OperationId = "StartNewConversation")]
        public async Task<ActionResult<int>> Start([FromBody] StartConversationRequest req)
        {
            var user = await _db.Users.FindAsync(req.UserId);
            if (user is null) return NotFound("User not found.");

            // expire any active conversations for this user
            var actives = await _db.Conversations
                .Where(c => c.UserId == req.UserId && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow))
                .ToListAsync();
            foreach (var c in actives) c.ExpiresAt = DateTime.UtcNow;

            // create a new one
            var conv = new Conversation { UserId = req.UserId, CreatedAt = DateTime.UtcNow };
            _db.Conversations.Add(conv);
            await _db.SaveChangesAsync();

            return Ok(conv.Id); 
        }

        // Optional helper to support CreatedAtAction
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "GetMessageById")]
        public async Task<ActionResult<MessageDto>> GetById(int id)
        {
            var m = await _db.Messages
                             .Include(x => x.User)
                             .FirstOrDefaultAsync(x => x.Id == id);
            if (m is null) return NotFound();

            var dto = new MessageDto(m.Id, m.UserId, m.Content, m.SentAt, m.User.Username);
            return Ok(dto);
        }
    }
}
