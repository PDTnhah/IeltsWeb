using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public interface IChoiceRepository
    {
        Task<Choice?> GetByIdAsync(long id);
        Task<Choice> AddAsync(Choice choice);
        Task UpdateAsync(Choice choice);
        Task DeleteAsync(long id);
        Task DeleteRangeAsync(IEnumerable<Choice> choices);
    }
}