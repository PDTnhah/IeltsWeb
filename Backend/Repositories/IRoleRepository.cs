using Backend.Models;
using System.Collections.Generic; // Cho List
using System.Threading.Tasks;    // Cho Task

namespace Backend.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> FindByIdAsync(long id); // Đổi sang async, cho phép null
        Task<Role?> FindByNameAsync(string name); // Thêm: Tìm role bằng tên
        Task<List<Role>> FindAllAsync();      // Đổi sang async
        Task<Role> SaveAsync(Role role);       // Đổi sang async
        Task<bool> ExistsAsync(long id);       // Thêm: Kiểm tra tồn tại bằng ID
        // Task<bool> DeleteByIdAsync(long id); // Tùy chọn: Nếu admin có quyền xóa role
    }
}