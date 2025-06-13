using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(long id); // Include Choices
        Task<IEnumerable<Question>> GetByLessionIdAsync(long lessionId);
        Task<Question> AddAsync(Question question);
        Task UpdateAsync(Question question); // Chỉ đánh dấu, SaveChanges ở Service
        Task DeleteAsync(long id);
        Task DeleteRangeAsync(IEnumerable<Question> questions); // Chỉ đánh dấu
        Task<int> CountByLessionIdAsync(long lessionId);
    }
}