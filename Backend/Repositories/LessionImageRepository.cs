using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Repositories
{
    public class LessionImageRepository : ILessionImageRepository
    {
        private readonly AppDbContext _context;

        public LessionImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<LessionImage> FindByLessionId(long id)
        {
            return _context.LessionImages
                .Where(i => i.lessionId == id)
                .ToList();
        }

        public LessionImage Save(LessionImage lessionImage)
        {
            if (lessionImage.id == 0)
            {
                _context.LessionImages.Add(lessionImage);
            }
            else
            {
                _context.LessionImages.Update(lessionImage);
            }
            _context.SaveChanges();
            return lessionImage;
        }
    }
}