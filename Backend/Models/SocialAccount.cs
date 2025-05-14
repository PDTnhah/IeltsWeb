using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("social_account")]
    public class SocialAccount
    {
        [Key]
        public long id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("provider")]
        public string provider { get; set; }

        [Required]
        [StringLength(500)]
        [Column("provider_id")]
        public string providerId { get; set; }

        [StringLength(100)]
        [Column("email")]
        public string email { get; set; }

        [StringLength(100)]
        [Column("user_name")]
        public string name { get; set; }
    }
}