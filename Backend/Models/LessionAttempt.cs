using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Backend.Models;

namespace Backend.Models
{
    [Table("lession_attempts")]
    public class LessionAttempt : BaseEntity // Giả sử BaseEntity có createdAt, updatedAt
    {
        [Key]
        public long id { get; set; }

        [Required]
        public long user_id { get; set; }
        [ForeignKey("user_id")]
        public virtual User User { get; set; }

        [Required]
        public long lession_id { get; set; }
        [ForeignKey("lession_id")]
        public virtual Lession Lession { get; set; }

        [Column(TypeName = "decimal(5, 2)")] // Ví dụ: 95.50
        public decimal? score { get; set; }

        public int? total_questions { get; set; }
        public int? correct_answers { get; set; }

        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public DateTime? completed_at { get; set; }

        public virtual ICollection<AnswerAttempt> AnswerAttempts { get; set; } = new List<AnswerAttempt>();
    }
}

