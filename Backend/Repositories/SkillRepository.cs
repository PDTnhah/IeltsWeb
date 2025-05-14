using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly AppDbContext _context;

        public SkillRepository(AppDbContext context)
        {
            _context = context;
        }

        public Skill Save(Skill skill)
        {
            if (skill.id == 0)
            {
                _context.Skills.Add(skill);
            }
            else
            {
                _context.Skills.Update(skill);
            }
            _context.SaveChanges();
            return skill;
        }

        public Skill FindById(long id)
        {
            return _context.Skills.FirstOrDefault(s => s.id == id);
        }

        public List<Skill> FindAll()
        {
            return _context.Skills.ToList();
        }

        public void DeleteById(long id)
        {
            var skill = _context.Skills.FirstOrDefault(s => s.id == id);
            if (skill != null)
            {
                _context.Skills.Remove(skill);
                _context.SaveChanges();
            }
        }
    }
}