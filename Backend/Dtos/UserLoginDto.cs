using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Phone number is required")]
        public string phoneNumber { get; set; }

        [Required(ErrorMessage = "Password cannot be blank")]
        public string password { get; set; }
    }
}