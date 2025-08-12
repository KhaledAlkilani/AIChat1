using AIChat1.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using server.Hubs;
using Swashbuckle.AspNetCore.Annotations;

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

        public ChatController(IHubContext<ChatHub> chatHub, AppDbContext db)
        {
            _chatHub = chatHub;
            _db = db;
        }

        // POST /api/chat/send
        [HttpPost("send")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
        [SwaggerOperation(OperationId = "SendMessage")] // this becomes the generated client method name
        public async Task<ActionResult<MessageDto>> Send([FromBody] SendMessageRequest req)
        {
            // 1) Validate user exists
            var user = await _db.Users.FindAsync(req.UserId);
            if (user is null) return NotFound($"User {req.UserId} not found.");

            // 2) Persist message
            var entity = new Message
            {
                UserId = req.UserId,
                Content = req.Content,
                SentAt = DateTime.UtcNow
            };
            _db.Messages.Add(entity);
            await _db.SaveChangesAsync();

            // 3) Map to response DTO
            var dto = new MessageDto(entity.Id, entity.UserId, entity.Content, entity.SentAt, user.Username);

            // 4) Broadcast to SignalR clients: (sender, text) to match your client handler
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", user.Username, dto.Content);

            // 5) Return 201 with the created resource
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        // Optional helper to support CreatedAtAction
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "GetMessageById")]
        public async Task<ActionResult<MessageDto>> GetById(int id)
        {
            var m = await _db.Messages
                             .Include(x => x.Username)
                             .FirstOrDefaultAsync(x => x.Id == id);
            if (m is null) return NotFound();

            var dto = new MessageDto(m.Id, m.UserId, m.Content, m.SentAt, m.Username.Username);
            return Ok(dto);
        }
    }
}
