using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Backend.Models;
using Backend.Dtos;
using Backend.Service;
using Backend.Exceptions;

namespace Backend.Controller
{
    [ApiController]
    [Route("api/v1/skills")]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpPost]
        public IActionResult CreateSkill([FromBody] SkillDtos skillDtos)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(errors);
            }

            try
            {
                var newSkill = _skillService.CreateSkill(skillDtos);
                return Ok(newSkill);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllSkills()
        {
            var skills = _skillService.GetAllSkills();
            return Ok(skills);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSkill(long id, [FromBody] SkillDtos skillDtos)
        {
            try
            {
                var updatedSkill = _skillService.UpdateSkill(id, skillDtos);
                return Ok("Update skill successfully");
            }
            catch (DataNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSkill(long id)
        {
            try
            {
                _skillService.DeleteSkill(id);
                return Ok($"Delete skill with id = {id} successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}