using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("tokens")]
    public class Token
    {
        [Key]
        public long id { get; set; }

        [StringLength(225)]
        [Column("token_value")]
        public string tokenValue { get; set; }

        [StringLength(50)]
        [Column("token_type")]
        public string tokenType { get; set; }

        [Column("expiration_date")]
        public DateTime expirationDate { get; set; }

        public bool expired { get; set; }

        public bool revoked { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public long userId { get; set; }

        public User user { get; set; }
    }
}