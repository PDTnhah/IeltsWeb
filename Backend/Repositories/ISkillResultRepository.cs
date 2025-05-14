using Backend.Models;
using System.Collections.Generic;

namespace Backend.Repositories
{
    public interface ISkillResultRepository
    {
        List<SkillResult> FindByUserId(long userId);
        SkillResult Save(SkillResult skillResult);
    }
}