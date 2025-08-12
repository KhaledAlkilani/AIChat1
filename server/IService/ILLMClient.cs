namespace AIChat1.IService
{
    public interface ILLMClient
    {
        Task<string> GetChatCompletionAsync(string userName, string userMessage, CancellationToken ct = default);
    }
} 
