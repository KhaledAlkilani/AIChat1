using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AIChat1.IService;
using AIChat1.Options;
using Microsoft.Extensions.Options;

namespace AIChat1.Services
{
    public sealed class OpenAiClient : ILLMClient
    {
        private readonly HttpClient _http;
        private readonly OpenAIOptions _opt;

        private static readonly JsonSerializerOptions J = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public OpenAiClient(HttpClient http, IOptions<OpenAIOptions> opt)
        {
            _http = http;
            _opt = opt.Value;

            // Safe idempotent header setup
            if (!_http.DefaultRequestHeaders.Contains("Authorization"))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _opt.ApiKey);

            // BaseAddress is set in Program.cs
        }

        public async Task<string> GetChatCompletionAsync(string userName, string userMessage, CancellationToken ct = default)
        {
            // Minimal request payload for Chat Completions
            var payload = new
            {
                model = _opt.Model, // e.g. "gpt-4o-mini"
                messages = new object[]
                {
                    new { role = "system", content = "You are an assistant in a desktop AI chat app." },
                    new { role = "user",   content = $"{userName}: {userMessage}" }
                }
            };

            using var resp = await _http.PostAsJsonAsync("v1/chat/completions", payload, J, ct);
            resp.EnsureSuccessStatusCode();

            using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync(ct), cancellationToken: ct);

            // Extract first choice content: choices[0].message.content
            var content = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();

            return content ?? "";
        }
    }
}
