using AIChat1.Entity;

namespace AIChat1.IRepository
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetAsync(int id, CancellationToken ct = default);
        Task<List<Message>> GetHistoryAsync(int conversationId, CancellationToken ct = default);
        Task<Conversation> AddAsync(Conversation c, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
