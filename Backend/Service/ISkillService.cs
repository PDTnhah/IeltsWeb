using System.Collections.Generic;
using Backend.Models;
using Backend.Dtos;

namespace Backend.Service
{
    public interface ISkillService
    {
        Skill CreateSkill(SkillDtos skillDto);
        Skill GetSkillById(long id);
        List<Skill> GetAllSkills();
        Skill UpdateSkill(long skillId, SkillDtos skillDto);
        void DeleteSkill(long id);
    }
}