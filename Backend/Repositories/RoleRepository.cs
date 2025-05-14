using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public Role FindById(long id)
        {
            return _context.Roles.FirstOrDefault(r => r.id == id);
        }

        public Role Save(Role role)
        {
            if (role.id == 0)
            {
                _context.Roles.Add(role);
            }
            else
            {
                _context.Roles.Update(role);
            }
            _context.SaveChanges();
            return role;
        }
    }
}