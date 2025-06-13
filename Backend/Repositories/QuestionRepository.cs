using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;
        public QuestionRepository(AppDbContext context) { _context = context; }

        public async Task<Question?> GetByIdAsync(long id)
        {
            return await _context.Questions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.id == id);
        }

        public async Task<IEnumerable<Question>> GetByLessionIdAsync(long lessionId)
        {
            return await _context.Questions
                .Where(q => q.lession_id == lessionId)
                .Include(q => q.Choices)
                .OrderBy(q => q.order_index)
                .ToListAsync();
        }

        public async Task<Question> AddAsync(Question question)
        {
            question.createdAt = DateTime.UtcNow;
            question.updatedAt = DateTime.UtcNow;
            await _context.Questions.AddAsync(question);
            // Không SaveChangesAsync ở đây nếu Service dùng transaction và gọi SaveChanges ở cuối transaction
            return question;
        }

        public Task UpdateAsync(Question question)
        {
            question.updatedAt = DateTime.UtcNow;
            _context.Questions.Update(question);
            // Không SaveChangesAsync ở đây
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(long id)
        {
            // Lấy question cùng choices để EF Core có thể xử lý cascade delete nếu được cấu hình
            var question = await _context.Questions.Include(q => q.Choices).FirstOrDefaultAsync(q => q.id == id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                // Không SaveChangesAsync ở đây
            }
        }

        public Task DeleteRangeAsync(IEnumerable<Question> questions)
        {
            // Tương tự, cần đảm bảo choices được load nếu cascade không tự động từ DB
            // Hoặc DB đã cấu hình ON DELETE CASCADE cho khóa ngoại từ Choices đến Questions
             _context.Questions.RemoveRange(questions);
             // Không SaveChangesAsync ở đây
             return Task.CompletedTask;
        }

        public async Task<int> CountByLessionIdAsync(long lessionId)
        {
            return await _context.Questions.CountAsync(q => q.lession_id == lessionId);
        }
    }
}