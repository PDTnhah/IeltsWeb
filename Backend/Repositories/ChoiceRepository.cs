using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class ChoiceRepository : IChoiceRepository
    {
        private readonly AppDbContext _context;
        public ChoiceRepository(AppDbContext context) { _context = context; }

        public async Task<Choice?> GetByIdAsync(long id)
        {
            return await _context.Choices.FindAsync(id);
        }

        public async Task<Choice> AddAsync(Choice choice)
        {
            await _context.Choices.AddAsync(choice);
            return choice;
        }

        public Task UpdateAsync(Choice choice)
        {
            _context.Choices.Update(choice);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(long id)
        {
            var choice = await GetByIdAsync(id);
            if (choice != null) _context.Choices.Remove(choice);
        }
        public Task DeleteRangeAsync(IEnumerable<Choice> choices)
        {
             _context.Choices.RemoveRange(choices);
             return Task.CompletedTask;
        }
    }
}