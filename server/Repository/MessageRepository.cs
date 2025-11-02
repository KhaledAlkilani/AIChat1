using AIChat1.Entity;
using AIChat1.IRepository;

namespace AIChat1.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _db;
        public MessageRepository(AppDbContext db) => _db = db;

        public async Task<Message> AddAsync(Message m, CancellationToken ct = default)
        {
            _db.Messages.Add(m);
            await _db.SaveChangesAsync(ct);
            return m;
        }
    }
}