using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool ExistsByPhoneNumber(string phoneNumber)
        {
            return _context.Users.Any(u => u.phoneNumber == phoneNumber);
        }

        public User FindByPhoneNumber(string phoneNumber)
        {
            return _context.Users
                .Include(u => u.role)
                .FirstOrDefault(u => u.phoneNumber == phoneNumber);
        }

        public User Save(User user)
        {
            if (user.id == 0)
            {
                _context.Users.Add(user);
            }
            else
            {
                _context.Users.Update(user);
            }
            _context.SaveChanges();
            return user;
        }
    }
}