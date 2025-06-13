using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class LessionRepository : ILessionRepository
    {
        private readonly AppDbContext _context;

        public LessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Lession> SaveAsync(Lession lession)
        {
            if (lession.id == 0) // Tạo mới
            {
                lession.createdAt = DateTime.UtcNow; // Gán thời gian tạo
                lession.updatedAt = DateTime.UtcNow; // Gán thời gian cập nhật
                await _context.Lessions.AddAsync(lession);
            }
            else // Cập nhật
            {
                var existingLession = await _context.Lessions.FindAsync(lession.id);
                if (existingLession != null) {
                    // Chỉ cập nhật các trường cần thiết, không attach lại toàn bộ object
                    // Hoặc dùng _context.Entry(existingLession).CurrentValues.SetValues(lession);
                    // rồi chỉ định các navigation property không thay đổi.
                    // Cách an toàn hơn là gán từng trường:
                    existingLession.name = lession.name;
                    existingLession.thumbnail = lession.thumbnail;
                    existingLession.description = lession.description;
                    existingLession.main_content = lession.main_content;
                    existingLession.audio_url = lession.audio_url;
                    existingLession.transcript = lession.transcript;
                    existingLession.skillId = lession.skillId;
                    // existingLession.is_published = lession.is_published; // ĐÃ BỎ
                    existingLession.updatedAt = DateTime.UtcNow; // Cập nhật thời gian
                    _context.Lessions.Update(existingLession);
                } else {
                     throw new KeyNotFoundException($"Lession with id {lession.id} not found for update.");
                }
            }
            await _context.SaveChangesAsync();
            return lession;
        }

        public async Task<Lession?> FindByIdAsync(long id)
        {
            return await _context.Lessions
                                 .Include(l => l.skill)
                                 .FirstOrDefaultAsync(l => l.id == id);
        }

        public async Task<Lession?> GetByIdWithDetailsAsync(long id)
        {
            return await _context.Lessions
                .Include(l => l.skill)
                .Include(l => l.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(l => l.id == id);
        }

        public async Task<(List<Lession> Lessions, int TotalRecords)> FindAllPaginatedAsync(long? skillId, int page, int limit)
        {
            // Đảm bảo page và limit hợp lệ
            if (page < 1) page = 1;
            if (limit < 1) limit = 10; // Hoặc một giá trị mặc định khác

            var query = _context.Lessions.Include(l => l.skill).AsQueryable();

            if (skillId.HasValue)
            {
                query = query.Where(l => l.skillId == skillId.Value);
            }
            // Không còn lọc theo is_published

            var totalRecords = await query.CountAsync();
            var lessions = await query.OrderByDescending(l => l.createdAt)
                                      .Skip((page - 1) * limit)
                                      .Take(limit)
                                      .ToListAsync();
            return (lessions, totalRecords);
        }

        public async Task<int> CountAsync(long? skillId = null)
        {
            var query = _context.Lessions.AsQueryable();
            if (skillId.HasValue)
            {
                query = query.Where(l => l.skillId == skillId.Value);
            }
            // Không còn lọc theo is_published
            return await query.CountAsync();
        }

        public async Task DeleteAsync(Lession lession)
        {
            _context.Lessions.Remove(lession);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, long? currentIdToExclude = null)
        {
            if (currentIdToExclude.HasValue)
            {
                return await _context.Lessions.AnyAsync(l => l.name == name && l.id != currentIdToExclude.Value);
            }
            return await _context.Lessions.AnyAsync(l => l.name == name);
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Lessions.AnyAsync(l => l.id == id);
        }
    }
}