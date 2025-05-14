using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("skillResult")]
    public class SkillResult
    {
        [Key]
        public long id { get; set; }

        [Column("score")]
        public float score { get; set; }

        [Column("completed_at")]
        public DateTime completedAt { get; set; }

        [ForeignKey("Skill")]
        [Column("skill_id")]
        public long skillId { get; set; }

        public Skill skill { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public long userId { get; set; }

        public User user { get; set; }
    }
}