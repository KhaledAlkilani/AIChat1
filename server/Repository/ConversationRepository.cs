using AIChat1.Entity;
using AIChat1.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AIChat1.Repository
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _db;
        public ConversationRepository(AppDbContext db) => _db = db;

        public Task<Conversation?> GetAsync(int id, CancellationToken ct = default)
            => _db.Conversations.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id, ct);

        public Task<List<Message>> GetHistoryAsync(int conversationId, CancellationToken ct = default)
            => _db.Messages
                   .Include(m => m.User)
                   .Where(m => m.ConversationId == conversationId)
                   .OrderBy(m => m.SentAt)
                   .ToListAsync(ct);

        public async Task<Conversation> AddAsync(Conversation c, CancellationToken ct = default)
        {
            _db.Conversations.Add(c);
            await _db.SaveChangesAsync(ct);
            return c;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var c = await _db.Conversations.FindAsync([id], ct);
            if (c is null) return;
            _db.Conversations.Remove(c);
            await _db.SaveChangesAsync(ct);
        }
    }
}
