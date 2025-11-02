using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using AIChat1.IService;

namespace server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatSvc;
        public ChatHub(IChatService chatSvc) => _chatSvc = chatSvc;

        // Client calls: connection.invoke('SendMessage', message)
        public async Task SendMessage(string message)
        {
            if (Context.User?.Identity?.IsAuthenticated != true)
                throw new HubException("Unauthenticated");

            // Prefer Name claim; fall back to Identity.Name
            var userName =
                Context.User.FindFirstValue(ClaimTypes.Name)
                ?? Context.User.Identity!.Name
                ?? "Unknown";

            // Echo the user's message back only to them
            await Clients.Caller.SendAsync("ReceiveMessage", userName, message);

            // Ask the AI and reply only to the same caller
            var aiReply = await _chatSvc.GetAiResponseAsync(userName, message);
            await Clients.Caller.SendAsync("ReceiveMessage", "AI", aiReply);
        }

        public override async Task OnConnectedAsync()
        {
            // Optional: put this connection in a per-user group (useful later)
            var userId =
                Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context.User?.FindFirstValue(ClaimTypes.Name) // fallback
                ?? Context.ConnectionId;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId =
                Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context.User?.FindFirstValue(ClaimTypes.Name)
                ?? Context.ConnectionId;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
