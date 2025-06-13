using System;

namespace Backend.Dtos
{
    public class UserResponseDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? FacebookAccountId { get; set; }
        public int? GoogleAccountId { get; set; }
        public long RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
