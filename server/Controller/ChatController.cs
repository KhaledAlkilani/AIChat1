using AIChat1.DTOs;
using AIChat1.Entity;
using AIChat1.Entity.Enums;
using AIChat1.Helpers;
using AIChat1.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using server.Hubs;
using Swashbuckle.AspNetCore.Annotations;

namespace AIChat1.Controller
{
    public record SendMessageRequest(int UserId, string Content, int? ConversationId);

    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly AppDbContext _db;
        private readonly IChatService _chatSvc;
        private readonly ILLMClient _llmSvc;

        public record StartConversationRequest(int UserId);

        public ChatController(
            IHubContext<ChatHub> chatHub,
            AppDbContext db,
            IChatService chatSvc,
            ILLMClient llmSvc
        )
        {
            _chatHub = chatHub;
            _db = db;
            _chatSvc = chatSvc;
            _llmSvc = llmSvc;
        }

        // POST /api/chat/send
        [HttpPost("send")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
        [SwaggerOperation(OperationId = "SendMessage")] // this becomes the generated client method name
        public async Task<ActionResult<MessageDto>> Send([FromBody] SendMessageRequest req)
        {
            // 1) resolve user
            var user = await _db.Users.FindAsync(req.UserId);
            if (user is null)
                return NotFound($"User {req.UserId} not found.");

            // 2) get or create an active conversation for this user
            //var conv = await _db.Conversations
            //    .FirstOrDefaultAsync(c => c.UserId == req.UserId && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow));
            Conversation? conv = null;
            if (req.ConversationId is int cid)
                conv = await _db.Conversations.FirstOrDefaultAsync(c =>
                    c.Id == cid && c.UserId == req.UserId
                );

            if (conv is null)
                conv = await _db.Conversations.FirstOrDefaultAsync(c =>
                    c.UserId == req.UserId && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow)
                );

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
                Sender = MessageSender.User,
            };
            _db.Messages.Add(entity);

            if (string.IsNullOrWhiteSpace(conv.Title))
            {
                conv.Title = req.Content.Length > 60 ? req.Content[..60] : req.Content;
                _db.Conversations.Update(conv);
            }

            await _db.SaveChangesAsync();

            // 4) DTO + broadcast
            var dto = entity.ToDto();

            await _chatHub
                .Clients.User(user.Id.ToString())
                .SendAsync("ReceiveMessage", user.Username, dto.Content);

            // 5) Build short conversation history and ask the LLM
            string? aiReply = null;
            try
            {
                aiReply = await _llmSvc.GetReplyAsync(
                    user.Username,
                    req.Content,
                    HttpContext.RequestAborted
                );
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

                await _chatHub
                    .Clients.User(user.Id.ToString())
                    .SendAsync("ReceiveMessage", "AI", aiReply);
            }

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        // POST /api/chat/start
        [HttpPost("start")]
        [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "StartNewConversation")]
        public async Task<ActionResult<int>> Start([FromBody] StartConversationRequest req)
        {
            var user = await _db.Users.FindAsync(req.UserId);
            if (user is null)
                return NotFound("User not found.");

            // expire any active conversations for this user
            var actives = await _db
                .Conversations.Where(c =>
                    c.UserId == req.UserId && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow)
                )
                .ToListAsync();
            foreach (var c in actives)
                c.ExpiresAt = DateTime.UtcNow;

            // create a new one
            var conv = new Conversation { UserId = req.UserId, CreatedAt = DateTime.UtcNow };
            _db.Conversations.Add(conv);
            await _db.SaveChangesAsync();

            return Ok(conv.ToDto());
        }

        // Optional helper to support CreatedAtAction
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "GetMessageById")]
        public async Task<ActionResult<MessageDto>> GetById(int id)
        {
            var m = await _db.Messages.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (m is null)
                return NotFound();

            var dto = new MessageDto(
                m.Id,
                m.ConversationId,
                m.UserId,
                m.User.Username,
                m.Sender,
                m.Content,
                m.SentAt
            );

            return Ok(dto);
        }

        [HttpGet("conversations")]
        [ProducesResponseType(typeof(List<ConversationDto>), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "GetConversations")]
        public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConversations(
            [FromQuery] int userId
        )
        {
            var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound("User not found.");

            var conversations = await _db
                .Conversations.Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Title = c.Title,
                    CreatedAt = c.CreatedAt,
                    ExpiresAt = c.ExpiresAt,
                })
                .ToListAsync();

            return Ok(conversations);
        }

        // GET /api/chat/conversations/{conversationId}/messages
        [HttpGet("conversations/{conversationId:int}/messages")]
        [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
        // This OperationId is important! It generates the client-side function name.
        [SwaggerOperation(OperationId = "GetConversationMessages")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetConversationMessages(
            int conversationId
        )
        {
            // 1. (Optional) Check if the conversation exists
            var conversationExists = await _db.Conversations.AnyAsync(c => c.Id == conversationId);
            if (!conversationExists)
            {
                return NotFound($"Conversation {conversationId} not found.");
            }

            // 2. Fetch all messages for this conversation
            var messages = await _db
                .Messages.Where(m => m.ConversationId == conversationId)
                .Include(m => m.User) // We MUST include User to get the Username for the DTO
                .OrderBy(m => m.SentAt) // Order by date, oldest first, for chat history
                .Select(m => m.ToDto())
                .ToListAsync();

            return Ok(messages);
        }

        [HttpDelete("conversations/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(OperationId = "DeleteConversation")]
        public async Task<ActionResult> DeleteConversation(int id)
        {
            var existingConversation = await _db.Conversations.FirstOrDefaultAsync(c => c.Id == id);

            if (existingConversation == null)
            {
                // Use NotFound (404) for a missing resource
                return NotFound($"Conversation {id} not found!");
            }

            // 1. Find and remove all child messages first.
            var messages = await _db.Messages.Where(m => m.ConversationId == id).ToListAsync();

            if (messages.Any())
            {
                _db.Messages.RemoveRange(messages);
            }

            // 2. Now, remove the parent conversation
            _db.Conversations.Remove(existingConversation);

            // 3. Save all changes
            await _db.SaveChangesAsync();

            // 4. Return NoContent (204) as requested.
            // This is a standard and efficient way to confirm a successful DELETE.
            return NoContent();
        }
    }
}
