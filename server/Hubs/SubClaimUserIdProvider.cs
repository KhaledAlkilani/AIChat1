using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

public sealed class SubClaimUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        var user = connection.User;
        if (user is null) return null;

        // Prefer NameIdentifier if you add it later, else fall back to JWT `sub`
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
}
