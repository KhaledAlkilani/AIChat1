using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using AIChat1.IService;

namespace server.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatSvc;
        public ChatHub(IChatService chatSvc) { _chatSvc = chatSvc; }

        // Sends a message to all connected clients
        public async Task SendMessage(string userName, string message)
        {

            //var userName = Context.User?.Identity?.Name
            //           ?? throw new HubException("Unauthenticated");

            // echo back to caller
            await Clients.Caller.SendAsync("ReceiveMessage", userName, message);

            // get AI reply
            var aiReply = await _chatSvc.GetAiResponseAsync(userName, message);

            // send AI reply only to caller
            await Clients.Caller.SendAsync("ReceiveMessage", "AI", aiReply);
        }

        // Called when a client disconnects from the hub
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
