using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Thêm using này
using Backend.Models;
using Backend.Dtos;
using Backend.Repositories;
using Backend.Exceptions;
using AutoMapper; // Nếu bạn muốn map Skill sang SkillResponseDto

namespace Backend.Service
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;
        private readonly IMapper _mapper; // Inject IMapper nếu bạn muốn trả về DTO

        public SkillService(ISkillRepository skillRepository, IMapper mapper) // Thêm IMapper
        {
            _skillRepository = skillRepository;
            _mapper = mapper; // Gán mapper
        }

        public async Task<Skill> CreateSkillAsync(SkillDtos skillDto)
        {
            // Kiểm tra xem skill với tên này đã tồn tại chưa
            if (await _skillRepository.ExistsByNameAsync(skillDto.name))
            {
                throw new InvalidParamException($"Skill with name '{skillDto.name}' already exists.");
            }

            var newSkill = new Skill // Model Skill
            {
                name = skillDto.name,
                // createdAt và updatedAt sẽ được gán trong Repository hoặc BaseEntity
            };

            return await _skillRepository.SaveAsync(newSkill);
            // Nếu muốn trả về DTO:
            // var createdSkill = await _skillRepository.SaveAsync(newSkill);
            // return _mapper.Map<SkillResponseDto>(createdSkill); // Cần tạo SkillResponseDto và mapping
        }

        public async Task<Skill?> GetSkillByIdAsync(long id)
        {
            var skill = await _skillRepository.FindByIdAsync(id);
            if (skill == null)
            {
                // throw new DataNotFoundException($"Skill with id {id} not found"); // Hoặc trả về null tùy thiết kế
                return null;
            }
            return skill;
            // Nếu muốn trả về DTO:
            // return _mapper.Map<SkillResponseDto>(skill);
        }

        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            return await _skillRepository.FindAllAsync();
            // Nếu muốn trả về List<SkillResponseDto>:
            // var skills = await _skillRepository.FindAllAsync();
            // return _mapper.Map<List<SkillResponseDto>>(skills);
        }

        public async Task<Skill?> UpdateSkillAsync(long skillId, SkillDtos skillDto)
        {
            var existingSkill = await _skillRepository.FindByIdAsync(skillId);
            if (existingSkill == null)
            {
                // throw new DataNotFoundException($"Skill with id {skillId} not found for update.");
                return null; // Hoặc throw exception
            }

            // Kiểm tra nếu tên mới bị trùng với skill khác (trừ chính nó)
            if (existingSkill.name != skillDto.name && await _skillRepository.ExistsByNameAsync(skillDto.name, skillId))
            {
                 throw new InvalidParamException($"Another skill with name '{skillDto.name}' already exists.");
            }

            existingSkill.name = skillDto.name;
            // updatedAt sẽ được gán trong Repository hoặc BaseEntity

            return await _skillRepository.SaveAsync(existingSkill);
            // Nếu muốn trả về DTO:
            // var updatedSkill = await _skillRepository.SaveAsync(existingSkill);
            // return _mapper.Map<SkillResponseDto>(updatedSkill);
        }

        public async Task<bool> DeleteSkillAsync(long id)
        {
            return await _skillRepository.DeleteByIdAsync(id);
        }
    }
}