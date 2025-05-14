using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Repositories
{
    public class SkillResultRepository : ISkillResultRepository
    {
        private readonly AppDbContext _context;

        public SkillResultRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<SkillResult> FindByUserId(long userId)
        {
            return _context.SkillResults
                .Where(sr => sr.userId == userId)
                .ToList();
        }

        public SkillResult Save(SkillResult skillResult)
        {
            if (skillResult.id == 0)
            {
                _context.SkillResults.Add(skillResult);
            }
            else
            {
                _context.SkillResults.Update(skillResult);
            }
            _context.SaveChanges();
            return skillResult;
        }
    }
}