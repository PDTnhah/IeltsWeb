using Microsoft.AspNetCore.Mvc;
using Backend.Models; 
using Backend.Dtos;  
using Backend.Service;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic; 
using Backend.Exceptions; 

namespace Backend.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/skills")]
    // [Authorize(Roles = "Admin")]    
    public class AdminSkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public AdminSkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSkillAdmin([FromBody] SkillDtos skillDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newSkill = await _skillService.CreateSkillAsync(skillDto);
                return CreatedAtAction(nameof(GetSkillByIdAdmin), new { id = newSkill.id }, newSkill);
            }
            catch (InvalidParamException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log lỗi ex.Message
                return StatusCode(500, new { message = "An error occurred while creating the skill."});
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillByIdAdmin(long id)
        {
            var skill = await _skillService.GetSkillByIdAsync(id);
            if (skill == null)
            {
                return NotFound(new { message = $"Skill with id {id} not found" });
            }
            return Ok(skill);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkillsAdmin()
        {
            var skills = await _skillService.GetAllSkillsAsync();
            return Ok(skills); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkillAdmin(long id, [FromBody] SkillDtos skillDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var updatedSkill = await _skillService.UpdateSkillAsync(id, skillDto);
                if (updatedSkill == null)
                {
                    return NotFound(new { message = $"Skill with id {id} not found for update." });
                }
                return Ok(updatedSkill);
            }
            catch (InvalidParamException ex)
            {
                 return Conflict(new { message = ex.Message });
            }
            catch (DataNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the skill."});
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkillAdmin(long id)
        {
            var success = await _skillService.DeleteSkillAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Skill with id {id} not found for deletion." });
            }
            return Ok(new { message = $"Skill with id = {id} deleted successfully" }); // Hoặc NoContent()
        }
    }
}
