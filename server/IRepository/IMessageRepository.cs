using AIChat1.Entity;

namespace AIChat1.IRepository
{
    public interface IMessageRepository
    {
        Task<Message> AddAsync(Message m, CancellationToken ct = default);
    }
}
