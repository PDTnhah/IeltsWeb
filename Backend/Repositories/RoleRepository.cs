using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Cho List
using System.Linq;                // Cho FirstOrDefaultAsync, ToListAsync
using System.Threading.Tasks;    // Cho Task
using System;                   // Cho DateTime

namespace Backend.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> FindByIdAsync(long id)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.id == id);
        }

        public async Task<Role?> FindByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return await _context.Roles.FirstOrDefaultAsync(r => r.name.ToLower() == name.ToLower());
        }

        public async Task<List<Role>> FindAllAsync()
        {
            return await _context.Roles.OrderBy(r => r.name).ToListAsync();
        }

        public async Task<Role> SaveAsync(Role role)
        {
            // Giả sử Role model không có createdAt/updatedAt hoặc không kế thừa BaseEntity
            // Nếu có, bạn cần xử lý chúng tương tự như SkillRepository hoặc UserRepository
            if (role.id == 0) // Tạo mới
            {
                // Nếu Role có createdAt/updatedAt, gán ở đây:
                // if (role is BaseEntity be) { be.createdAt = DateTime.UtcNow; be.updatedAt = DateTime.UtcNow; }
                await _context.Roles.AddAsync(role);
            }
            else // Cập nhật
            {
                var existingRole = await _context.Roles.FindAsync(role.id);
                if (existingRole != null)
                {
                    existingRole.name = role.name;
                    // if (existingRole is BaseEntity be) { be.updatedAt = DateTime.UtcNow; }
                    _context.Roles.Update(existingRole);
                }
                else
                {
                    // Xử lý trường hợp role không tồn tại để cập nhật
                    throw new KeyNotFoundException($"Role with id {role.id} not found for update.");
                }
            }
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Roles.AnyAsync(r => r.id == id);
        }
    }
}