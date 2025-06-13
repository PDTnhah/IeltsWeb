using Backend.Models;      // Cho Model User
using Backend.Dtos;        // Cho UserDtos
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Service
{
    public interface IUserService
    {
        // Trả về Model User để có thể lấy password hash cho AuthService
        // Hoặc tạo một UserAuthDto riêng chỉ chứa id, email, password_hash, role
        Task<User> CreateUserAsync(UserDtos userDtos); // Nhận UserDtos, trả về Model User (bao gồm ID đã tạo)

        // Trả về Model User để Service khác có thể dùng, hoặc map sang UserResponseDto ở Controller nếu cần
        Task<User?> GetUserByIdAsync(long id);
        Task<User?> GetUserByEmailForAuthAsync(string email); // Cho AuthService
        Task<User?> UpdateUserByEmailAsync(string email, UserDtos userDtos);

        // Các hàm cho Admin quản lý User
        // Trả về danh sách User models, Controller sẽ map sang DTO nếu cần
        Task<(IEnumerable<User> Users, int TotalRecords, int TotalPages)> GetAllUsersPaginatedAsync(int page, int limit, string? searchTerm = null, long? roleId = null);
        Task<User?> UpdateUserByAdminAsync(long userId, UserDtos userDtos); // Admin cập nhật, nhận UserDtos
        Task<bool> DeleteUserByAdminAsync(long userId);

        Task<bool> UserExistsAsync(long id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    }
}