using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Repositories
{
    public class LessionRepository : ILessionRepository
    {
        private readonly AppDbContext _context;

        public LessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool ExistsByName(string name)
        {
            return _context.Lessions.Any(l => l.name == name);
        }

        public List<Lession> FindAll(int page, int limit)
        {
            return _context.Lessions
                .Include(l => l.skill)
                .OrderByDescending(l => l.createdAt)
                .Skip(page * limit)
                .Take(limit)
                .ToList();
        }

        public int Count()
        {
            return _context.Lessions.Count();
        }

        public Lession Save(Lession lession)
        {
            if (lession.id == 0)
            {
                _context.Lessions.Add(lession);
            }
            else
            {
                _context.Lessions.Update(lession);
            }
            _context.SaveChanges();
            return lession;
        }

        public Lession FindById(long id)
        {
            return _context.Lessions
                .Include(l => l.skill)
                .FirstOrDefault(l => l.id == id);
        }

        public void Delete(Lession lession)
        {
            _context.Lessions.Remove(lession);
            _context.SaveChanges();
        }
    }
}