using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Dtos
{
    public class UserDtos
    {
        public string fullName { get; set; }

        // [Required(ErrorMessage = "Phone number is required")]
        public string? phoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; }

        public string address { get; set; }

        [Required(ErrorMessage = "Password cannot be blank")]
        public string password { get; set; }

        public string reTypePassword { get; set; }

        [JsonPropertyName("date_of_birth")]
        public DateTime? dateOfBirth { get; set; }

        [JsonPropertyName("facebook_account_id")]
        public int facebookAccountId { get; set; }

        [JsonPropertyName("google_account_id")]
        public int googleAccountId { get; set; }

        [JsonPropertyName("role_id")]
        public long roleId { get; set; }
    }
}