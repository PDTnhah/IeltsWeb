using Backend.Dtos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Cho IFormFile

namespace Backend.Service
{
    public class DeepseekService : IDeepseekService
    {
        private readonly HttpClient _httpClient; // Đã được cấu hình BaseAddress và API Key mặc định
        private readonly IConfiguration _configuration;
        // private readonly ISpeechToTextService _speechToTextService; // TODO: Inject nếu có STT

        public DeepseekService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<AiFeedbackResponseDto> GetFeedbackAsync(AiFeedbackRequestDto requestDto)
        {
            string inputTextForAi;

            if (requestDto.FeedbackType.Equals("writing", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(requestDto.TextContent))
                {
                    return new AiFeedbackResponseDto { Success = false, ErrorMessage = "Text content is required for writing feedback." };
                }
                inputTextForAi = requestDto.TextContent;
            }
            else if (requestDto.FeedbackType.Equals("speaking", StringComparison.OrdinalIgnoreCase))
            {
                if (requestDto.AudioFile == null || requestDto.AudioFile.Length == 0)
                {
                    return new AiFeedbackResponseDto { Success = false, ErrorMessage = "Audio file is required for speaking feedback." };
                }
                try
                {
                    inputTextForAi = await ProcessAudioFileAsync(requestDto.AudioFile); // Placeholder, cần STT thật
                    if (string.IsNullOrWhiteSpace(inputTextForAi))
                    {
                        return new AiFeedbackResponseDto { Success = false, ErrorMessage = "Failed to convert audio to text or audio is empty." };
                    }
                }
                catch (Exception sttEx)
                {
                    Console.WriteLine($"Speech-to-text error: {sttEx.Message}");
                    return new AiFeedbackResponseDto { Success = false, ErrorMessage = $"Speech-to-text conversion failed: {sttEx.Message}" };
                }
            }
            else
            {
                return new AiFeedbackResponseDto { Success = false, ErrorMessage = "Invalid feedback type." };
            }

            // Xây dựng prompt cho DeepSeek
            // Bạn có thể tùy chỉnh systemPrompt và userMessageContent chi tiết hơn
            string systemPrompt = "You are an expert IELTS examiner. Your task is to provide detailed and constructive feedback on the provided IELTS submission. " +
                                  "Please analyze the content based on standard IELTS marking criteria: " +
                                  "For WRITING (Task Achievement/Response, Coherence and Cohesion, Lexical Resource, Grammatical Range and Accuracy). " +
                                  "For SPEAKING (Fluency and Coherence, Lexical Resource, Grammatical Range and Accuracy, Pronunciation). " +
                                  "Identify specific errors, explain why they are errors, and suggest concrete improvements. " +
                                  "If possible, provide an estimated overall band score and scores for each criterion. " +
                                  "Structure your feedback clearly with headings for each aspect (e.g., Grammar, Vocabulary, Suggestions, Estimated Score).";

            string userMessageContent = $"IELTS Submission Analysis Request:\n" +
                                        $"Type: IELTS {requestDto.FeedbackType}\n";
            if (!string.IsNullOrEmpty(requestDto.TaskType))
            {
                userMessageContent += $"Task Details: {requestDto.TaskType}\n";
            }
            if (!string.IsNullOrEmpty(requestDto.PromptOrQuestion))
            {
                 userMessageContent += $"Original Prompt/Question Given to Student: {requestDto.PromptOrQuestion}\n";
            }
            userMessageContent += $"Student's Submitted Content:\n---\n{inputTextForAi}\n---\n" +
                                  $"Please provide your detailed feedback as an IELTS examiner.";

            var openRouterRequestPayload = new OpenRouterChatRequestDto
            {
                model = _configuration["OpenRouter:DeepSeekModelName"] ?? "deepseek/deepseek-prover-v2:free", // Sử dụng model mới
                messages = new List<OpenRouterChatMessageDto>
                {
                    new OpenRouterChatMessageDto { role = "system", content = systemPrompt },
                    new OpenRouterChatMessageDto { role = "user", content = userMessageContent }
                },
                temperature = 0.6, // Điều chỉnh nhiệt độ để cân bằng giữa sáng tạo và chính xác
                max_tokens = 2048 // Tăng max_tokens để có phản hồi chi tiết
                // Thêm các tham số khác nếu model hỗ trợ và bạn muốn dùng (ví dụ: top_p)
            };

            // Tạo HttpRequestMessage để có thể tùy chỉnh headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "chat/completions"); // Endpoint vẫn là chat/completions
            requestMessage.Content = JsonContent.Create(openRouterRequestPayload, options: new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }); // Gửi payload với snake_case

            // Thêm các header tùy chọn như trong ví dụ Python
            var siteUrl = _configuration["YourSiteUrl"];
            var siteName = _configuration["YourSiteName"];

            if (!string.IsNullOrEmpty(siteUrl))
            {
                // "HTTP-Referer" là một header chuẩn, không cần TryAddWithoutValidation nếu giá trị hợp lệ
                requestMessage.Headers.Referrer = new Uri(siteUrl);
            }
            if (!string.IsNullOrEmpty(siteName))
            {
                requestMessage.Headers.TryAddWithoutValidation("X-Title", siteName);
            }
            // API Key đã được thêm vào DefaultRequestHeaders của HttpClient trong Program.cs

            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var openRouterResponse = JsonSerializer.Deserialize<OpenRouterChatCompletionDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (openRouterResponse?.choices != null && openRouterResponse.choices.Any() && openRouterResponse.choices[0].message != null)
                    {
                        return new AiFeedbackResponseDto
                        {
                            Success = true,
                            FeedbackText = openRouterResponse.choices[0].message.content
                        };
                    }
                    else if (openRouterResponse?.error != null)
                    {
                        Console.WriteLine($"OpenRouter API Error in JSON: {openRouterResponse.error.message} (Type: {openRouterResponse.error.type})");
                        return new AiFeedbackResponseDto { Success = false, ErrorMessage = $"OpenRouter API Error: {openRouterResponse.error.message}" };
                    }
                    Console.WriteLine($"OpenRouter No Valid Choices in JSON: {responseContent}");
                    return new AiFeedbackResponseDto { Success = false, ErrorMessage = "No valid response content from AI." };
                }
                else
                {
                    Console.WriteLine($"OpenRouter API request failed. Status: {response.StatusCode}");
                    Console.WriteLine($"OpenRouter API Error Content: {responseContent}");
                    try {
                        var errorResponse = JsonSerializer.Deserialize<OpenRouterChatCompletionDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (errorResponse?.error != null) {
                            return new AiFeedbackResponseDto { Success = false, ErrorMessage = $"AI API Error ({response.StatusCode}): {errorResponse.error.message}" };
                        }
                    } catch (JsonException) {
                         // Response không phải JSON, có thể là HTML lỗi
                    }
                    return new AiFeedbackResponseDto { Success = false, ErrorMessage = $"AI API request failed with status {response.StatusCode}. Check server logs for response content." };
                }
            }
            catch (HttpRequestException httpEx) {
                Console.WriteLine($"HttpRequestException calling OpenRouter API: {httpEx.ToString()}");
                return new AiFeedbackResponseDto { Success = false, ErrorMessage = $"Network error communicating with AI service: {httpEx.Message}" };
            }
            catch (TaskCanceledException timeoutEx) {
                Console.WriteLine($"TimeoutException calling OpenRouter API: {timeoutEx.ToString()}");
                return new AiFeedbackResponseDto { Success = false, ErrorMessage = "The request to the AI service timed out." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Generic exception calling OpenRouter API: {ex.ToString()}");
                return new AiFeedbackResponseDto { Success = false, ErrorMessage = $"An unexpected error occurred: {ex.Message}" };
            }
        }

        // Placeholder - Cần triển khai Speech-to-Text thật sự
        private async Task<string> ProcessAudioFileAsync(IFormFile audioFile)
        {
            Console.WriteLine($"Processing audio file: {audioFile.FileName}, size: {audioFile.Length} bytes, type: {audioFile.ContentType}");
            if (audioFile.Length > 10 * 1024 * 1024) // Giới hạn 10MB
            {
                 throw new Exception("Audio file is too large for STT processing (max 10MB).");
            }
            if (!audioFile.ContentType.StartsWith("audio/")) // Kiểm tra sơ bộ
            {
                throw new Exception("Invalid audio file type.");
            }

            // Đây là nơi bạn sẽ tích hợp dịch vụ STT
            // Ví dụ:
            // using var memoryStream = new MemoryStream();
            // await audioFile.CopyToAsync(memoryStream);
            // memoryStream.Position = 0;
            // var transcription = await _yourSttServiceClient.TranscribeAsync(memoryStream, audioFile.ContentType);
            // return transcription;

            await Task.Delay(500); // Giả lập
            return $"This is a simulated transcription of the audio file '{audioFile.FileName}'. The actual content would be the spoken words.";
        }
    }
}