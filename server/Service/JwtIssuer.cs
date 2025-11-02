using AIChat1;
using AIChat1.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public sealed class JwtIssuer : IJwtIssuer
{
    private readonly JwtSettings _opt;
    private readonly SigningCredentials _creds;

    public JwtIssuer(IOptions<JwtSettings> opt)
    {
        _opt = opt.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SecretKey));
        _creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public string Issue(int userId, string username, IEnumerable<string>? roles = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (roles != null)
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_opt.ExpMinutes),
            signingCredentials: _creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
