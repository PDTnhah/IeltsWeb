using Backend.Models;
using System.Collections.Generic;

namespace Backend.Repositories
{
    public interface ISkillRepository
    {
        Skill Save(Skill skill);
        Skill FindById(long id);
        List<Skill> FindAll();
        void DeleteById(long id);
    }
}