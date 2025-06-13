using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class AnswerAttemptRepository : IAnswerAttemptRepository
    {
        private readonly AppDbContext _context;

        public AnswerAttemptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AnswerAttempt?> GetByIdAsync(long id)
        {
            return await _context.AnswerAttempts
                                 .Include(aa => aa.Question)
                                 .Include(aa => aa.SelectedChoice)
                                 .FirstOrDefaultAsync(aa => aa.id == id);
        }

        public async Task<IEnumerable<AnswerAttempt>> GetByLessionAttemptIdAsync(long lessionAttemptId)
        {
            return await _context.AnswerAttempts
                                 .Where(aa => aa.lession_attempt_id == lessionAttemptId)
                                 .Include(aa => aa.Question)
                                    .ThenInclude(q => q.Choices)
                                 .Include(aa => aa.SelectedChoice)
                                 .OrderBy(aa => aa.Question.order_index) // Sắp xếp theo thứ tự câu hỏi
                                 .ToListAsync();
        }

        public async Task<AnswerAttempt> AddAsync(AnswerAttempt answerAttempt)
        {
            // submitted_at được gán mặc định trong Model
            await _context.AnswerAttempts.AddAsync(answerAttempt);
            // Không SaveChangesAsync ở đây nếu Service dùng transaction
            return answerAttempt;
        }

        public async Task AddRangeAsync(IEnumerable<AnswerAttempt> answerAttempts)
        {
            // submitted_at được gán mặc định trong Model
            await _context.AnswerAttempts.AddRangeAsync(answerAttempts);
            // Không SaveChangesAsync ở đây
        }
    }
}