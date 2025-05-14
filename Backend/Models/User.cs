using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("users")]
    public class User : BaseEntity
    {
        [Key]
        public long id { get; set; }

        [StringLength(100)]
        public string? fullname { get; set; }

        [StringLength(10)]
        [Column("phone_number")]
        public string? phoneNumber { get; set; }

        [StringLength(200)]
        [Column("address")]
        public string? address { get; set; }

        [Required]
        [StringLength(100)]
        [Column("email")]
        public string email { get; set; }

        [Required]
        [StringLength(200)]
        [Column("password")]
        public string password { get; set; }

        [Column("is_active")]
        public bool active { get; set; }

        [Column("date_of_birth")]
        public DateTime? dateOfBirth { get; set; }

        [Column("facebook_account_id")]
        public int? facebookAccountId { get; set; }

        [Column("google_account_id")]
        public int? googleAccountId { get; set; }

        [ForeignKey("Role")]
        [Column("role_id")]
        public long roleId { get; set; }

        public Role role { get; set; }
    }
}