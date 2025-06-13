using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Thêm cho IEnumerable
using System.Linq;
using System.Threading.Tasks;    // Thêm cho Task
using System;                   // Cho DateTime

namespace Backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return false;
            return await _context.Users.AnyAsync(u => u.phoneNumber == phoneNumber);
        }

        public async Task<User?> FindByPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return null;
            return await _context.Users
                .Include(u => u.role) // Tên navigation property là 'role' trong User.cs
                .FirstOrDefaultAsync(u => u.phoneNumber == phoneNumber);
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            return await _context.Users
                .Include(u => u.role) // Tên navigation property là 'role' trong User.cs
                .FirstOrDefaultAsync(u => u.email.ToLower() == email.ToLower()); // So sánh không phân biệt hoa thường
        }

        public async Task<User?> FindByIdAsync(long id)
        {
            return await _context.Users
                .Include(u => u.role) // Tên navigation property là 'role' trong User.cs
                .FirstOrDefaultAsync(u => u.id == id);
        }


        public async Task<User> SaveAsync(User user)
        {
            if (user.id == 0) // Tạo mới
            {
                // User model của bạn kế thừa BaseEntity, nên createdAt/updatedAt sẽ được xử lý ở đó hoặc ở đây
                if (user is BaseEntity baseEntity) // Kiểm tra an toàn
                {
                    baseEntity.createdAt = DateTime.UtcNow;
                    baseEntity.updatedAt = DateTime.UtcNow;
                }
                await _context.Users.AddAsync(user);
            }
            else // Cập nhật
            {
                var existingUser = await _context.Users.FindAsync(user.id);
                if (existingUser != null)
                {
                    // Cập nhật các trường cho phép thay đổi
                    existingUser.fullname = user.fullname;
                    existingUser.phoneNumber = user.phoneNumber;
                    existingUser.address = user.address;
                    // Không cho phép đổi email qua đây (thường có quy trình riêng)
                    // Không cập nhật password qua đây (có API đổi mật khẩu riêng)
                    existingUser.active = user.active;
                    existingUser.dateOfBirth = user.dateOfBirth;
                    existingUser.roleId = user.roleId; // Cho phép admin đổi role
                    if (existingUser is BaseEntity baseUpdatableEntity)
                    {
                        baseUpdatableEntity.updatedAt = DateTime.UtcNow;
                    }
                     _context.Users.Update(existingUser);
                }
                else
                {
                    throw new KeyNotFoundException($"User with id {user.id} not found for update.");
                }
            }
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Users.AnyAsync(u => u.id == id);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return await _context.Users.AnyAsync(u => u.email.ToLower() == email.ToLower());
        }

        public async Task<(IEnumerable<User> Users, int TotalRecords)> FindAllPaginatedAsync(int page, int limit, string? searchTerm = null, long? roleId = null)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            var query = _context.Users.Include(u => u.role).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(u => (u.fullname != null && u.fullname.ToLower().Contains(term)) ||
                                         (u.email != null && u.email.ToLower().Contains(term)) ||
                                         (u.phoneNumber != null && u.phoneNumber.Contains(term)));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.roleId == roleId.Value);
            }

            var totalRecords = await query.CountAsync();
            var users = await query.OrderBy(u => u.fullname ?? u.email) // Sắp xếp
                                   .Skip((page - 1) * limit)
                                   .Take(limit)
                                   .ToListAsync();
            return (users, totalRecords);
        }

        public async Task<int> CountAllAsync(string? searchTerm = null, long? roleId = null)
        {
             var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(u => (u.fullname != null && u.fullname.ToLower().Contains(term)) ||
                                         (u.email != null && u.email.ToLower().Contains(term)) ||
                                         (u.phoneNumber != null && u.phoneNumber.Contains(term)));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.roleId == roleId.Value);
            }
            return await query.CountAsync();
        }
    }
}