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
        private readonly ILogger<ChatController> _logger;

        public record StartConversationRequest(int UserId);

        public ChatController(
            IHubContext<ChatHub> chatHub,
            AppDbContext db,
            IChatService chatSvc,
            ILogger<ChatController> logger
        )
        {
            _chatHub = chatHub;
            _db = db;
            _chatSvc = chatSvc;
            _logger = logger;
        }

        [HttpPost("send")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
        [SwaggerOperation(OperationId = "SendMessage")]
        public async Task<ActionResult<MessageDto>> Send([FromBody] SendMessageRequest req)
        {
            var ct = HttpContext.RequestAborted;

            // 1) user
            var user = await _db.Users.FindAsync(new object?[] { req.UserId }, ct);
            if (user is null) return NotFound($"User {req.UserId} not found.");

            // 2) conversation
            var conv = await GetOrCreateActiveConversation(req.UserId, req.ConversationId, ct);

            // 3) user message
            EnsureTitleFromFirstMessage(conv, req.Content);
            _db.Conversations.Update(conv);
            var entity = await AddUserMessageAsync(conv, req.UserId, req.Content, ct);

            // 4) broadcast user message
            var dto = entity.ToDto();
            await _chatHub.Clients.User(user.Id.ToString()).SendAsync("ReceiveMessage", user.Username, dto.Content, ct);

            // 5) AI
            var aiReply = await GetAiReplyAsync(conv.Id, user.Username, req.Content, ct);
            if (!string.IsNullOrWhiteSpace(aiReply))
            {
                await PersistAndBroadcastAssistantAsync(conv, req.UserId, aiReply!, user.Id.ToString(), ct);
            }

            // 6) result
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

        // ↓ NEW helpers to reduce complexity

        private async Task<Conversation> GetOrCreateActiveConversation(int userId, int? conversationId, CancellationToken ct)
        {
            if (conversationId is int cid)
            {
                var byId = await _db.Conversations.FirstOrDefaultAsync(c => c.Id == cid && c.UserId == userId, ct);
                if (byId is not null) return byId;
            }

            var active = await _db.Conversations
                .FirstOrDefaultAsync(c => c.UserId == userId && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow), ct);

            if (active is not null) return active;

            var conv = new Conversation { UserId = userId, CreatedAt = DateTime.UtcNow };
            _db.Conversations.Add(conv);
            await _db.SaveChangesAsync(ct);
            return conv;
        }

        private static void EnsureTitleFromFirstMessage(Conversation conv, string content)
        {
            if (!string.IsNullOrWhiteSpace(conv.Title)) return;
            conv.Title = content.Length > 60 ? content[..60] : content;
        }

        // Replace the signature and usage of AddUserMessageAsync to accept the required parameters instead of using 'req'
        private async Task<Message> AddUserMessageAsync(Conversation conv, int userId, string content, CancellationToken ct)
        {
            var entity = Mapper.NewUserMessage(conv.Id, userId, content);
            _db.Messages.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        private async Task<string?> GetAiReplyAsync(int conversationId, string username, string latestUserContent, CancellationToken ct)
        {
            try
            {
                var rows = await _db.Messages
                    .Where(m => m.ConversationId == conversationId)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync(ct);

                const int maxTurns = 20;
                var recent = rows.Count > maxTurns ? rows.Skip(rows.Count - maxTurns) : rows;

                var llmMsgs = new List<LlmMsg>
        {
            new("system", "You are an assistant in a desktop AI chat app. Be concise and helpful.")
        };

                foreach (var m in recent)
                {
                    var role = m.Sender == MessageSender.User ? "user" : "assistant";
                    var content = m.Sender == MessageSender.User ? $"{username}: {m.Content}" : m.Content;
                    llmMsgs.Add(new LlmMsg(role, content));
                }

                var withHistory = await _chatSvc.GetAiResponseWithHistoryAsync(llmMsgs, ct);
                if (!string.IsNullOrWhiteSpace(withHistory)) return withHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "History LLM call failed for conversation {ConversationId}", conversationId);
                // fall through to single-turn
            }

            try
            {
                return await _chatSvc.GetAiResponseAsync(username, latestUserContent, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Single-turn LLM call failed for conversation {ConversationId}", conversationId);
                return null;
            }
        }
        private async Task PersistAndBroadcastAssistantAsync(Conversation conv, int userId, string aiReply, string userSignalId, CancellationToken ct)
        {
            var aiMsg = Mapper.NewAssistantMessage(conv.Id, userId, aiReply);
            _db.Messages.Add(aiMsg);
            await _db.SaveChangesAsync(ct);

            await _chatHub.Clients.User(userSignalId).SendAsync("ReceiveMessage", "AI", aiReply, ct);
        }
    }
}
