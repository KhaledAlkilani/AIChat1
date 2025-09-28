namespace AIChat1.IService
{
    public record LlmMsg(string Role, string Content);
    public interface ILLMClient
    {
        Task<string?> GetReplyAsync(string userName, string userMessage, CancellationToken ct = default);

        Task<string?> GetReplyWithHistoryAsync(IEnumerable<LlmMsg> messages, CancellationToken ct = default);
    }
} 
