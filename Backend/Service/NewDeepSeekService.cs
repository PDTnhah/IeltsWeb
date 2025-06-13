using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.Service
{
    public class NewDeepSeekService
    {
        private readonly string? _apiKey;
        private readonly string? _openAiKey;
        private readonly HttpClient _httpClient;

        public NewDeepSeekService(IConfiguration config)
        {
            _apiKey = config["DeepSeekApiKey"];
            _openAiKey = config["OpenAIApiKey"];
            _httpClient = new HttpClient();
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new InvalidOperationException("DeepSeek API key is missing in configuration.");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            Console.WriteLine($"[Debug] Auth header: {_httpClient.DefaultRequestHeaders.Authorization}");


            // Optional headers for OpenRouter rankings
            // _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://your-site.com"); // <- thay thế
            // _httpClient.DefaultRequestHeaders.Add("X-Title", "IeltsWeb"); // <- thay thế
        }

        public async Task<string> CallDeepSeekAsync(string prompt)
        {
            var payload = new
            {
                model = "deepseek/deepseek-chat-v3-0324:free",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"DeepSeek API error: {response.StatusCode} - {error}");
            }

            var responseString = await response.Content.ReadAsStringAsync();

            // Extract nội dung trả về từ message
            using var jsonDoc = JsonDocument.Parse(responseString);
            var messageContent = jsonDoc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return messageContent ?? "No response content";
        }

        public async Task<string> TranscribeAudioAsync(Stream audioStream, string fileName)
        {
            using var form = new MultipartFormDataContent();
            var fileContent = new StreamContent(audioStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
            form.Add(fileContent, "file", fileName);
            form.Add(new StringContent("whisper-1"), "model");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/transcriptions")
            {
                Content = form
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKey);

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Whisper API error: {response.StatusCode} - {json}");

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("text").GetString() ?? string.Empty;
        }

    }
}