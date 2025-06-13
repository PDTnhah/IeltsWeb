using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks; // Thêm using này

namespace Backend.Repositories
{
    public interface ISkillRepository
    {
        Task<Skill> SaveAsync(Skill skill); // Đổi tên và thành async
        Task<Skill?> FindByIdAsync(long id); // Đổi tên, thành async, và cho phép null
        Task<List<Skill>> FindAllAsync();    // Đổi tên và thành async
        Task<bool> DeleteByIdAsync(long id); // Đổi tên, thành async, trả về bool để biết thành công hay không
        Task<bool> ExistsAsync(long id); // Thêm hàm kiểm tra tồn tại
        Task<bool> ExistsByNameAsync(string name, long? currentIdToExclude = null); // Kiểm tra trùng tên
    }
}