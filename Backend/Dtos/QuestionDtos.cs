using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Backend.Models; // Để dùng enum QuestionType

namespace Backend.Dtos
{
    public class ChoiceInputDto
    {
        public long? id { get; set; }
        [Required] public string choice_text { get; set; }
        public bool is_correct { get; set; } = false;
        public int order_index { get; set; } = 0;
    }

    public class QuestionInputDto // DTO để tạo/sửa Question
    {
        public long? id { get; set; }   
        [Required] public string question_text { get; set; }
        [Required] public QuestionType question_type { get; set; } = QuestionType.MultipleChoice;
        public string? explanation { get; set; }
        public int order_index { get; set; } = 0;
        // [Required] public long lession_id { get; set; } // Câu hỏi này thuộc Lession nào
        [Required(ErrorMessage = "Câu hỏi phải có ít nhất 2 lựa chọn.")]
        [MinLength(2, ErrorMessage = "Câu hỏi phải có ít nhất 2 lựa chọn.")]
        public List<ChoiceInputDto> choices { get; set; } = new List<ChoiceInputDto>();
    }

    // DTOs để trả về (Response)
    public class ChoiceResponseDto
    {
        public long id { get; set; }
        public string choice_text { get; set; }
        public bool is_correct { get; set; }
        public int order_index { get; set; }
    }

    public class QuestionResponseDto
    {
        public long id { get; set; }
        public string question_text { get; set; }
        public QuestionType question_type { get; set; }
        public string? explanation { get; set; }
        public int order_index { get; set; }
        public long lession_id { get; set; }
        public List<ChoiceResponseDto> choices { get; set; } = new List<ChoiceResponseDto>();
    }
}