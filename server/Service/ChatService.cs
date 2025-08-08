using AIChat1.IService;

namespace AIChat1.Services
{
    public class ChatService : IChatService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public ChatService(IConfiguration config, HttpClient http)
        {
            _config = config;
            _http = http;
        }
        public async Task<string> GetAiResponseAsync(string userName, string message)
        {
            // Here you’d call your AI API (OpenAI, Azure OpenAI, etc.)
            // For example (pseudocode):
            /*
            var request = new {
              model = "gpt-4",
              prompt = $"{userName}: {message}",
              max_tokens = 150
            };
            var response = await _http.PostAsJsonAsync(aiEndpoint, request);
            var content  = await response.Content.ReadFromJsonAsync<AiResponse>();
            return content.Choices.First().Text.Trim();
            */

            // Stub while you wire up real AI:
            await Task.Delay(50);
            return $"(AI echo) {message}";
        }
    }
}
