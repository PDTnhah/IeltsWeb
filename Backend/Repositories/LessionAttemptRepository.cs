using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class LessionAttemptRepository : ILessionAttemptRepository
    {
        private readonly AppDbContext _context;

        public LessionAttemptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LessionAttempt?> GetByIdAsync(long id)
        {
            return await _context.LessionAttempts
                                 .Include(la => la.Lession)
                                    .ThenInclude(l => l.skill) // Lấy Skill của Lession
                                 .Include(la => la.User) // Lấy thông tin User
                                 .FirstOrDefaultAsync(la => la.id == id);
        }

        public async Task<LessionAttempt?> GetByIdWithDetailsAsync(long id)
        {
            return await _context.LessionAttempts
                                 .Include(la => la.Lession)
                                    .ThenInclude(l => l.skill)
                                 .Include(la => la.User)
                                 .Include(la => la.AnswerAttempts) // Lấy các câu trả lời đã nộp
                                    .ThenInclude(aa => aa.Question) // Lấy thông tin câu hỏi của câu trả lời
                                        .ThenInclude(q => q.Choices) // Lấy các lựa chọn của câu hỏi đó
                                 .Include(la => la.AnswerAttempts) // Include lại AnswerAttempts để lấy SelectedChoice
                                    .ThenInclude(aa => aa.SelectedChoice) // Lấy lựa chọn đã chọn
                                 .FirstOrDefaultAsync(la => la.id == id);
        }


        public async Task<IEnumerable<LessionAttempt>> GetByUserIdAsync(long userId, int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            return await _context.LessionAttempts
                                 .Where(la => la.user_id == userId)
                                 .Include(la => la.Lession)
                                    .ThenInclude(l => l.skill)
                                 .OrderByDescending(la => la.completed_at ?? la.start_time ?? la.createdAt)
                                 .Skip((page - 1) * limit)
                                 .Take(limit)
                                 .ToListAsync();
        }

         public async Task<IEnumerable<LessionAttempt>> GetByLessionIdAsync(long lessionId, int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            return await _context.LessionAttempts
                                 .Where(la => la.lession_id == lessionId)
                                 .Include(la => la.User)
                                 .Include(la => la.Lession) // Thêm để có thể lấy tên Lession nếu cần
                                    .ThenInclude(l => l.skill)
                                 .OrderByDescending(la => la.completed_at ?? la.start_time ?? la.createdAt)
                                 .Skip((page - 1) * limit)
                                 .Take(limit)
                                 .ToListAsync();
        }


        public async Task<LessionAttempt> AddAsync(LessionAttempt attempt)
        {
            attempt.createdAt = DateTime.UtcNow;
            attempt.updatedAt = DateTime.UtcNow;
            await _context.LessionAttempts.AddAsync(attempt);
            // Không SaveChangesAsync ở đây nếu Service dùng transaction và gọi SaveChanges ở cuối transaction
            return attempt;
        }

        public Task UpdateAsync(LessionAttempt attempt) // Thay đổi để không trả về gì, chỉ đánh dấu
        {
            attempt.updatedAt = DateTime.UtcNow;
            _context.LessionAttempts.Update(attempt);
            // Không SaveChangesAsync ở đây
            return Task.CompletedTask;
        }

        public async Task<int> CountByUserIdAsync(long userId)
        {
            return await _context.LessionAttempts.CountAsync(la => la.user_id == userId);
        }

        public async Task<int> CountByLessionIdAsync(long lessionId)
        {
             return await _context.LessionAttempts.CountAsync(la => la.lession_id == lessionId);
        }
    }
}