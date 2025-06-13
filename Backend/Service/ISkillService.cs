using System.Collections.Generic;
using Backend.Models; // Trả về Model Skill
using Backend.Dtos;   // Dùng SkillDtos cho input
using System.Threading.Tasks; // Thêm using này

namespace Backend.Service
{
    public interface ISkillService
    {
        Task<Skill> CreateSkillAsync(SkillDtos skillDto); // Đổi tên và thành async
        Task<Skill?> GetSkillByIdAsync(long id);         // Đổi tên, thành async, cho phép null
        Task<List<Skill>> GetAllSkillsAsync();            // Đổi tên và thành async
        Task<Skill?> UpdateSkillAsync(long skillId, SkillDtos skillDto); // Đổi tên, thành async, cho phép null
        Task<bool> DeleteSkillAsync(long id);             // Đổi tên, thành async, trả về bool
    }
}