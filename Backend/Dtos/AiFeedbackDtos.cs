using Microsoft.AspNetCore.Http; // Cho IFormFile
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    // DTO cho request từ Frontend
    public class AiFeedbackRequestDto
    {
        [Required(ErrorMessage = "Feedback type is required (e.g., 'writing' or 'speaking').")]
        public string FeedbackType { get; set; } // "writing" hoặc "speaking"

        public string? TextContent { get; set; } // Cho IELTS Writing
        public IFormFile? AudioFile { get; set; } // Cho IELTS Speaking (file MP3)

        // Thêm các tham số khác nếu cần, ví dụ: task type (Task 1, Task 2), prompt cụ thể
        public string? TaskType { get; set; } // "Task 1", "Task 2", "Speaking Part 1/2/3"
        public string? PromptOrQuestion { get; set; } // Đề bài gốc (nếu có)
    }

    // DTO cho response từ API của bạn (có thể chỉ là string hoặc một cấu trúc phức tạp hơn)
    public class AiFeedbackResponseDto
    {
        public string FeedbackText { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // --- DTOs cho việc gọi OpenRouter API (nội bộ Service) ---
    public class OpenRouterChatMessageDto
    {
        public string role { get; set; } = "user"; // Hoặc "system"
        public string content { get; set; }
    }

    public class OpenRouterChatRequestDto
    {
        public string model { get; set; } = "deepseek/deepseek-prover-v2:free"; // Model DeepSeek, kiểm tra tên chính xác trên OpenRouter
        public List<OpenRouterChatMessageDto> messages { get; set; } = new List<OpenRouterChatMessageDto>();
        public double? temperature { get; set; } // Tùy chọn: 0.0 - 2.0
        public int? max_tokens { get; set; }    // Tùy chọn
        // Thêm các tham số khác của API OpenRouter nếu cần
    }

    public class OpenRouterChatChoiceDto
    {
        public OpenRouterChatMessageDto message { get; set; }
        // Thêm các trường khác như finish_reason nếu cần
    }

    public class OpenRouterChatCompletionDto // DTO để deserialize response từ OpenRouter
    {
        public string? id { get; set; }
        public string? model { get; set; }
        public List<OpenRouterChatChoiceDto>? choices { get; set; }
        // Thêm các trường khác như 'created', 'usage' nếu cần
        public OpenRouterError? error {get; set;} // Để bắt lỗi từ OpenRouter
    }
    public class OpenRouterError
    {
        public string? message { get; set; }
        public string? type { get; set; }
        // code, param...
    }
}