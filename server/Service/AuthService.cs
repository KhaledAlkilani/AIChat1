using AIChat1.Entity;
using AIChat1.Helpers;
using AIChat1.IRepository;
using AIChat1.IService;
using static AIChat1.DTOs.Auths;

namespace AIChat1.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IJwtIssuer _jwt; // add your existing issuer

        public AuthService(IUserRepository users, IJwtIssuer jwt)
        {
            _users = users; _jwt = jwt;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
        {
            var exists = await _users.GetByUsernameAsync(req.Username);
            if (exists != null) throw new InvalidOperationException("Username taken");

            var u = new User { Username = req.Username, HashedPassword = CustomPasswordHasher.HashPassword(req.Password) };
            u = await _users.AddAsync(u);

            var token = _jwt.Issue(u.Id, u.Username /*, roles: u.Roles?.Select(r=>r.Name) */);
            return new AuthResponse(token, u.Id, u.Username);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest req)
        {
            var u = await _users.GetByUsernameAsync(req.Username) ?? throw new UnauthorizedAccessException();
            if (!CustomPasswordHasher.VerifyPassword(req.Password, u.HashedPassword)) throw new UnauthorizedAccessException();
            var token = _jwt.Issue(u.Id, u.Username /*, roles: ... */);
            return new AuthResponse(token, u.Id, u.Username);
        }
    }
}
