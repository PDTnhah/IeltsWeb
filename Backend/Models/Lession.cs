using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("lessions")]
    public class Lession : BaseEntity
    {
        [Key]
        public long id { get; set; }

        [Required]
        [StringLength(350)]
        [Column("name")]
        public string name { get; set; }

        [StringLength(300)]
        [Column("thumbnail")]
        public string? thumbnail { get; set; }

        [Column("description")]
        public string? description { get; set; }

        [Column("main_content", TypeName = "LONGTEXT")]
        public string? main_content { get; set; }

        [StringLength(500)]
        [Column("audio_url")]
        public string? audio_url { get; set; }

        [Column("transcript", TypeName = "TEXT")]
        public string? transcript { get; set; }

        [ForeignKey("Skill")]
        [Column("skill_id")]
        public long skillId { get; set; }

        public Skill skill { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}