using AIChat1.IService;

namespace AIChat1.Services
{
    public class ChatService : IChatService
    {
        private readonly ILLMClient _llm;

        public ChatService(ILLMClient llm) => _llm = llm;

        public Task<string?> GetAiResponseAsync(string userName, string message, CancellationToken ct = default)
            => _llm.GetReplyAsync(userName, message, ct);

        public Task<string?> GetAiResponseWithHistoryAsync(IEnumerable<LlmMsg> messages, CancellationToken ct = default)
            => _llm.GetReplyWithHistoryAsync(messages, ct);
    }
}
