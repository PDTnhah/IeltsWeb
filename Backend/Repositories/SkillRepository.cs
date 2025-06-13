using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Cho DateTime nếu bạn có dùng ở đâu đó khác, nhưng không cho createdAt/updatedAt của Skill

namespace Backend.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly AppDbContext _context;

        public SkillRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Skill> SaveAsync(Skill skill)
        {
            if (skill.id == 0) // Tạo mới
            {
                // KHÔNG CÓ: skill.createdAt = DateTime.UtcNow;
                // KHÔNG CÓ: skill.updatedAt = DateTime.UtcNow;
                await _context.Skills.AddAsync(skill);
            }
            else // Cập nhật
            {
                var existingSkill = await _context.Skills.FindAsync(skill.id);
                if (existingSkill != null)
                {
                    existingSkill.name = skill.name;
                    // KHÔNG CÓ: existingSkill.updatedAt = DateTime.UtcNow;
                    _context.Skills.Update(existingSkill);
                }
                else
                {
                    // Xử lý trường hợp skill không tồn tại để cập nhật
                    // Có thể throw exception hoặc coi như tạo mới nếu logic cho phép
                    // Hiện tại, nếu không tìm thấy, Update sẽ không làm gì
                    // Để an toàn hơn, Service nên kiểm tra ExistsAsync trước khi gọi Update
                    _context.Skills.Update(skill); // EF Core sẽ theo dõi và update nếu tìm thấy
                }
            }
            await _context.SaveChangesAsync();
            return skill;
        }

        public async Task<Skill?> FindByIdAsync(long id)
        {
            return await _context.Skills.FirstOrDefaultAsync(s => s.id == id);
        }

        public async Task<List<Skill>> FindAllAsync()
        {
            return await _context.Skills.OrderBy(s => s.name).ToListAsync();
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.id == id);
            if (skill != null)
            {
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Skills.AnyAsync(s => s.id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name, long? currentIdToExclude = null)
        {
            if (currentIdToExclude.HasValue)
            {
                return await _context.Skills.AnyAsync(s => s.name == name && s.id != currentIdToExclude.Value);
            }
            return await _context.Skills.AnyAsync(s => s.name == name);
        }
    }
}