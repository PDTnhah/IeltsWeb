using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("choices")]
    public class Choice : BaseEntity
    {
        [Key]
        public long id { get; set; }

        [Required]
        [StringLength(500)]
        [Column("choice_text")] // Nội dung lựa chọn (A, B, C, D)
        public string choice_text { get; set; }

        public bool is_correct { get; set; } = false; // Đánh dấu đây là đáp án đúng

        public int order_index { get; set; } = 0; // Thứ tự của lựa chọn (A=0, B=1...)

        // Foreign Key tới Question
        [Required]
        public long question_id { get; set; }
        [ForeignKey("question_id")]
        public virtual Question Question { get; set; }
    }
}