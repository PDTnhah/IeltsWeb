using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [JsonPropertyName("email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [JsonPropertyName("password")]
        public string password { get; set; }
    }

    public class RegisterDto
    {
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [JsonPropertyName("name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [JsonPropertyName("email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [JsonPropertyName("password")]
        public string password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("password", ErrorMessage = "Passwords do not match")]
        [JsonPropertyName("confirm_password")]
        public string confirmPassword { get; set; }
    }

    public class AuthResponse
    {
        [JsonPropertyName("token")]
        public string token { get; set; }

        [JsonPropertyName("user_id")]
        public long userId { get; set; }
    }
}