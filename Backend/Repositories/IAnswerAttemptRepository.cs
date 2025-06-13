using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public interface IAnswerAttemptRepository
    {
        Task<AnswerAttempt?> GetByIdAsync(long id);
        Task<IEnumerable<AnswerAttempt>> GetByLessionAttemptIdAsync(long lessionAttemptId);
        Task<AnswerAttempt> AddAsync(AnswerAttempt answerAttempt);
        Task AddRangeAsync(IEnumerable<AnswerAttempt> answerAttempts);
        // Update/Delete thường không cần thiết cho từng AnswerAttempt riêng lẻ
    }
}