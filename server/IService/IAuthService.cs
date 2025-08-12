using static AIChat1.DTOs.Auths;

namespace AIChat1.IService
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest req);
        Task<AuthResponse> LoginAsync(LoginRequest req);
    }
}
