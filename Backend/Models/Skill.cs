using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("skills")]
    public class Skill
    {
        [Key]
        public long id { get; set; }

        [Required]
        [Column("name")]
        public string name { get; set; }

        public virtual ICollection<Lession> Lessions { get; set; } = new List<Lession>();

    }
}