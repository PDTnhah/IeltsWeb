using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public interface ILessionAttemptRepository
    {
        Task<LessionAttempt?> GetByIdAsync(long id);
        Task<LessionAttempt?> GetByIdWithDetailsAsync(long id); // Bao gồm Lession, User, AnswerAttempts và chi tiết của AnswerAttempts
        Task<IEnumerable<LessionAttempt>> GetByUserIdAsync(long userId, int page, int limit);
        Task<IEnumerable<LessionAttempt>> GetByLessionIdAsync(long lessionId, int page, int limit);
        Task<LessionAttempt> AddAsync(LessionAttempt attempt);
        Task UpdateAsync(LessionAttempt attempt); // Chỉ đánh dấu, SaveChanges ở Service
        Task<int> CountByUserIdAsync(long userId);
        Task<int> CountByLessionIdAsync(long lessionId);
        // Thêm các hàm khác nếu cần
    }
}