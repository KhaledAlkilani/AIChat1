using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using server.Hubs;

namespace AIChat1.Extensions
{
    public static class SignalREndpoints
    {
        // encapsulate all your hub mappings here
        public static void MapSignalRHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<ChatHub>("/chat");
            // if you ever add more Hubs:
            // endpoints.MapHub<NotificationHub>("/notify");
        }
    }
}