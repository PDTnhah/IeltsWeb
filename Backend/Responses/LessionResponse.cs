using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Backend.Dtos;

namespace Backend.Responses
{
     public class LessionListResponseItemDto // Đổi tên để rõ hơn
    {
        public long id { get; set; }
        public string name { get; set; }
        public string? thumbnail { get; set; }
        public string? description { get; set; }
        public long skillId { get; set; }
        public string skillName { get; set; } // Nên thêm tên skill
        // public bool is_published { get; set; } // Đã bỏ
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int questionCount { get; set; } // Số lượng câu hỏi
    }

    // DTO cho chi tiết một bài học (khi User bấm vào một bài học)
    public class LessionDetailViewDto : LessionListResponseItemDto // Kế thừa các trường chung
    {
        public string? main_content { get; set; } // Nội dung Reading
        public string? audio_url { get; set; }    // URL audio Listening
        public string? transcript { get; set; }   // Lời thoại Listening
        public List<QuestionResponseDto> questions { get; set; } = new List<QuestionResponseDto>();
    }

    // DTO để đóng gói response cho API GET /lessions (phân trang)
    public class PaginatedLessionResponse
    {
        public List<LessionListResponseItemDto> lessions { get; set; }
        public int totalPages { get; set; }
        public int currentPage { get; set; }
        public int totalRecords { get; set; }
    }
    public class LessionResponse : BaseResponse
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(350, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 350 characters")]
        public string name { get; set; }

        [StringLength(300)]
        public string thumbnail { get; set; }

        public string description { get; set; }

        [JsonPropertyName("skill_id")]
        public long skillId { get; set; }
    }
}