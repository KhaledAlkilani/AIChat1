using AIChat1.IService;

namespace AIChat1.Services
{
    public class ChatService : IChatService
    {
        private readonly ILLMClient _llm;

        public ChatService(ILLMClient llm) => _llm = llm;

        public Task<string> GetAiResponseAsync(string userName, string message, CancellationToken ct = default)
            => _llm.GetChatCompletionAsync(userName, message, ct);
    }
}
