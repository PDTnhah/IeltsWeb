using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Dtos;      
using Backend.Service; 
using System.Threading.Tasks;
using Backend.Responses;
using Backend.Exceptions;

namespace Backend.Controller
{
    [Route("api/v1/admin/lessions")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminLessionController : ControllerBase
    {
        private readonly ILessionService _lessionService;

        public AdminLessionController(ILessionService lessionService)
        {
            _lessionService = lessionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLessionWithQuestions([FromBody] LessionDtos lessionInputDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newLessionDetail = await _lessionService.CreateLessionWithQuestionsAsync(lessionInputDto);
                return CreatedAtAction(nameof(GetLessionDetailForAdmin), new { id = newLessionDetail.id }, newLessionDetail);
            }
            catch (DataNotFoundException ex) { return BadRequest(new { message = ex.Message }); }
            catch (InvalidParamException ex) { return BadRequest(new { message = ex.Message });}
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AdminLessionController.CreateLessionWithQuestions: {ex.ToString()}");
                return StatusCode(500, new { message = "An error occurred while creating the lession." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLessionWithQuestions(long id, [FromBody] LessionDtos lessionInputDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var updatedLessionDetail = await _lessionService.UpdateLessionWithQuestionsAsync(id, lessionInputDto);
                if (updatedLessionDetail == null)
                {
                    return NotFound(new { message = $"Lession with ID {id} not found." });
                }
                return Ok(updatedLessionDetail);
            }
            catch (DataNotFoundException ex) { return BadRequest(new { message = ex.Message }); }
            catch (InvalidParamException ex) { return BadRequest(new { message = ex.Message }); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AdminLessionController.UpdateLessionWithQuestions: {ex.ToString()}");
                return StatusCode(500, new { message = "An error occurred while updating the lession." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLessionsForAdmin([FromQuery] long? skillId, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;
            var result = await _lessionService.GetLessionsPaginatedAsync(skillId, page, limit);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessionDetailForAdmin(long id)
        {
            var lession = await _lessionService.GetLessionDetailByIdAsync(id);
            if (lession == null)
            {
                return NotFound(new { message = "Lesson not found." });
            }
            return Ok(lession);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLessionAdmin(long id)
        {
            try
            {
                var success = await _lessionService.DeleteLessionAsync(id);
                if (!success) return NotFound(new { message = $"Lession with ID {id} not found." });
                return Ok(new { message = "Lession deleted successfully" });
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error in AdminLessionController.DeleteLessionAdmin: {ex.ToString()}");
                return StatusCode(500, new { message = "An error occurred while deleting the lession." });
            }
        }
    }
}