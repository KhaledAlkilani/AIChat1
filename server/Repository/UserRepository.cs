using AIChat1.Entity;
using AIChat1.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AIChat1.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

        public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
            => _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
            => _db.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

        public async Task<User> AddAsync(User user, CancellationToken ct = default)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            return user;
        }
    }
}