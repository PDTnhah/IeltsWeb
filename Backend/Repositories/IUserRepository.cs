using Backend.Models;
using System.Collections.Generic; // Thêm cho IEnumerable
using System.Threading.Tasks;    // Thêm cho Task

namespace Backend.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber); // Đổi sang async
        Task<User?> FindByPhoneNumberAsync(string phoneNumber);  // Đổi sang async, cho phép null
        Task<User?> FindByEmailAsync(string email);             // Thêm: Tìm user bằng email (quan trọng cho login)
        Task<User?> FindByIdAsync(long id);                     // Thêm: Tìm user bằng ID
        Task<User> SaveAsync(User user);                       // Đổi sang async
        Task<bool> ExistsAsync(long id);                        // Thêm: Kiểm tra user tồn tại bằng ID
        Task<bool> ExistsByEmailAsync(string email);            // Thêm: Kiểm tra user tồn tại bằng email

        // Các hàm cho Admin quản lý User (ví dụ)
        Task<(IEnumerable<User> Users, int TotalRecords)> FindAllPaginatedAsync(int page, int limit, string? searchTerm = null, long? roleId = null);
        Task<int> CountAllAsync(string? searchTerm = null, long? roleId = null);
        // Task DeleteAsync(User user); // Nếu admin có quyền xóa user
    }
}