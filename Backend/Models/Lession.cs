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
        public string thumbnail { get; set; }

        [Column("description")]
        public string description { get; set; }

        [ForeignKey("Skill")]
        [Column("skill_id")]
        public long skillId { get; set; }

        public Skill skill { get; set; }
    }
}