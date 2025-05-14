using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        public long id { get; set; }

        [StringLength(100)]
        [Column("name")]
        public string name { get; set; }
    }
}