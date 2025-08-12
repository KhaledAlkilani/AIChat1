namespace AIChat1.DTOs
{
    public class Auths
    {
        public record RegisterRequest(string Username, string Password);
        public record LoginRequest(string Username, string Password);
        public record AuthResponse(string Token, int UserId, string Username);
    }
}
