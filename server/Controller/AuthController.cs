using AIChat1;
using AIChat1.DTOs;
using AIChat1.Entity;
using AIChat1.Helpers;
using AIChat1.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtIssuer _jwt;

    public AuthController(AppDbContext db, IJwtIssuer jwt)
    {
        _db = db; _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == req.Username);
        if (user is null || !CustomPasswordHasher.VerifyPassword(req.Password, user.HashedPassword))
            return Unauthorized();

        var token = _jwt.Issue(user.Id, user.Username);
        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest req)
    {
        var exists = await _db.Users.AnyAsync(u => u.Username == req.Username);
        if (exists) return Conflict("Username already exists.");

        var user = new User
        {
            Username = req.Username,
            HashedPassword = CustomPasswordHasher.HashPassword(req.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Register), new { id = user.Id }, new { user.Id, user.Username });
    }
}
