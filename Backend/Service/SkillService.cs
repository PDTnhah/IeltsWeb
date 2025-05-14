using System;
using System.Collections.Generic;
using Backend.Models;
using Backend.Dtos;
using Backend.Repositories;
using Backend.Exceptions;

namespace Backend.Service
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;

        public SkillService(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public Skill CreateSkill(SkillDtos skillDto)
        {
            var newSkill = new Skill
            {
                name = skillDto.name
            };

            return _skillRepository.Save(newSkill);
        }

        public Skill GetSkillById(long id)
        {
            return _skillRepository.FindById(id)
                ?? throw new DataNotFoundException($"Skill with id {id} not found");
        }

        public List<Skill> GetAllSkills()
        {
            return _skillRepository.FindAll();
        }

        public Skill UpdateSkill(long skillId, SkillDtos skillDto)
        {
            var existingSkill = GetSkillById(skillId);
            existingSkill.name = skillDto.name;

            return _skillRepository.Save(existingSkill);
        }

        public void DeleteSkill(long id)
        {
            _skillRepository.DeleteById(id);
        }
    }
}