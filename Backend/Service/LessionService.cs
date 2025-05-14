using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Models;
using Backend.Dtos;
using Backend.Responses;
using Backend.Repositories;
using Backend.Exceptions;

namespace Backend.Service
{
    public class LessionService : ILessionService
    {
        private readonly ILessionRepository _lessionRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly ILessionImageRepository _lessionImageRepository;

        public LessionService(
            ILessionRepository lessionRepository,
            ISkillRepository skillRepository,
            ILessionImageRepository lessionImageRepository)
        {
            _lessionRepository = lessionRepository;
            _skillRepository = skillRepository;
            _lessionImageRepository = lessionImageRepository;
        }

        public Lession CreateLession(LessionDtos lessionDtos)
        {
            var existingSkill = _skillRepository.FindById(lessionDtos.skillId)
                ?? throw new DataNotFoundException($"Cannot find skill with id: {lessionDtos.skillId}");

            var newLession = new Lession
            {
                name = lessionDtos.name,
                thumbnail = lessionDtos.thumbnail,
                description = lessionDtos.description,
                skill = existingSkill
            };

            return _lessionRepository.Save(newLession);
        }

        public Lession GetLessionById(long id)
        {
            return _lessionRepository.FindById(id)
                ?? throw new DataNotFoundException($"Cannot find lession with id: {id}");
        }

        public List<LessionResponse> GetAllLessions(int page, int limit)
        {
            var lessions = _lessionRepository.FindAll(page, limit);
            return lessions.Select(l => new LessionResponse
            {
                name = l.name,
                thumbnail = l.thumbnail,
                description = l.description,
                skillId = l.skill.id,
                createdAt = l.createdAt,
                updatedAt = l.updatedAt
            }).ToList();
        }

        public int GetLessionCount()
        {
            return _lessionRepository.Count();
        }

        public Lession UpdateLession(long id, LessionDtos lessionDtos)
        {
            var existingLession = GetLessionById(id);
            var existingSkill = _skillRepository.FindById(lessionDtos.skillId)
                ?? throw new DataNotFoundException($"Cannot find skill with id: {lessionDtos.skillId}");

            existingLession.name = lessionDtos.name;
            existingLession.thumbnail = lessionDtos.thumbnail;
            existingLession.description = lessionDtos.description;
            existingLession.skill = existingSkill;
            existingLession.OnUpdate();

            return _lessionRepository.Save(existingLession);
        }

        public void DeleteLession(long id)
        {
            var lession = _lessionRepository.FindById(id);
            if (lession != null)
            {
                _lessionRepository.Delete(lession);
            }
        }

        public bool ExistsByName(string name)
        {
            return _lessionRepository.ExistsByName(name);
        }

        public LessionImage CreateLessionImage(long id, LessionImageDto lessionImageDto)
        {
            var existingLession = _lessionRepository.FindById(id)
                ?? throw new DataNotFoundException($"Cannot find lession with id: {id}");

            var imageCount = _lessionImageRepository.FindByLessionId(id).Count;
            if (imageCount >= LessionImage.maximumImagesPerLession)
            {
                throw new InvalidParamException($"Number of images must be <= {LessionImage.maximumImagesPerLession}");
            }

            var newLessionImage = new LessionImage
            {
                lession = existingLession,
                imageUrl = lessionImageDto.imageUrl
            };

            return _lessionImageRepository.Save(newLessionImage);
        }
    }
}