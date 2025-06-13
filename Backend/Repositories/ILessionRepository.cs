using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public interface ILessionRepository
    {
        Task<Lession> SaveAsync(Lession lession); // Đổi tên và thành async
        Task<Lession?> FindByIdAsync(long id);    // Đổi tên và thành async
        Task<Lession?> GetByIdWithDetailsAsync(long id); // Lấy Lession cùng Questions và Choices
        // FindAll cũ có thể bỏ nếu FindAllPaginatedAsync đáp ứng đủ nhu cầu
        // Task<List<Lession>> FindAllAsync(int page, int limit);
        Task<(List<Lession> Lessions, int TotalRecords)> FindAllPaginatedAsync(long? skillId, int page, int limit); // Không còn publishedOnly
        Task<int> CountAsync(long? skillId = null); // Không còn publishedOnly, đổi tên Count
        Task DeleteAsync(Lession lession); // Đổi tên và thành async
        Task<bool> ExistsByNameAsync(string name, long? currentIdToExclude = null); // Thêm currentIdToExclude
        Task<bool> ExistsAsync(long id);
    }
}