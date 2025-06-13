using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("answer_attempts")]
    public class AnswerAttempt // Không cần BaseEntity nếu submitted_at là đủ
    {
        [Key]
        public long id { get; set; }

        [Required]
        public long lession_attempt_id { get; set; }
        [ForeignKey("lession_attempt_id")]
        public virtual LessionAttempt LessionAttempt { get; set; }

        [Required]
        public long question_id { get; set; }
        [ForeignKey("question_id")]
        public virtual Question Question { get; set; }

        public long? selected_choice_id { get; set; } // For multiple choice
        [ForeignKey("selected_choice_id")]
        public virtual Choice? SelectedChoice { get; set; }

        [Column(TypeName = "TEXT")]
        public string? user_answer_text { get; set; } // For fill-in-the-blank, etc.

        public bool? is_correct { get; set; } // Calculated on submission

        public DateTime submitted_at { get; set; } = DateTime.UtcNow;
    }
}
