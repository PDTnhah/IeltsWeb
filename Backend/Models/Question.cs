using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Backend.Models
{
    public enum QuestionType // Đặt enum này ở một nơi chung nếu nhiều model dùng
    {
        MultipleChoice, // Trắc nghiệm nhiều lựa chọn
        TrueFalse,
        FillInTheBlank
        // Thêm các loại khác nếu cần
    }

    [Table("questions")]
    public class Question : BaseEntity
    {
        [Key]
        public long id { get; set; }

        [Required]
        [Column("question_text", TypeName = "TEXT")] // Nội dung câu hỏi
        public string question_text { get; set; }

        [Required]
        public QuestionType question_type { get; set; } = QuestionType.MultipleChoice;

        [Column("explanation", TypeName = "TEXT")] // Giải thích đáp án (tùy chọn)
        public string? explanation { get; set; }

        public int order_index { get; set; } = 0; // Thứ tự câu hỏi trong bài học

        // Foreign Key tới Lession
        [Required]
        public long lession_id { get; set; }
        [ForeignKey("lession_id")]
        public virtual Lession Lession { get; set; } // Câu hỏi này thuộc về bài học nào

        // Danh sách các lựa chọn cho câu hỏi này (chỉ áp dụng cho MultipleChoice)
        public virtual ICollection<Choice> Choices { get; set; } = new List<Choice>();
    }
}