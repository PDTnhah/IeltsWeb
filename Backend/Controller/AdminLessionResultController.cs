using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Dtos; 
using Backend.Service;
using System.Threading.Tasks;
using System;

namespace Backend.Controller
{
    [Route("api/v1/admin/lession-results")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminLessionResultController : ControllerBase
    {
        private readonly ILessionResultService _resultService;
        public AdminLessionResultController(ILessionResultService resultService)
        {
            _resultService = resultService;
        }

        [HttpGet("lession/{lessionId}/attempts")] // GET api/v1/admin/lession-results/lession/1/attempts
        public async Task<IActionResult> GetAttemptsForLession(long lessionId, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;
            try
            {
                var (attempts, totalRecords, totalPages) = await _resultService.GetAttemptsForLessionByAdminAsync(lessionId, page, limit);
                return Ok(new { Data = attempts, TotalRecords = totalRecords, TotalPages = totalPages, CurrentPage = page });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAttemptsForLession (Admin): {ex.ToString()}");
                return StatusCode(500, new { message = "Error fetching attempts for lession." });
            }
        }

        [HttpGet("user/{userId}/attempts")] // GET api/v1/admin/lession-results/user/5/attempts
        public async Task<IActionResult> GetAttemptsForUser(long userId, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;
            try
            {
                 var (attempts, totalRecords, totalPages) = await _resultService.GetAttemptsForUserByAdminAsync(userId, page, limit);
                return Ok(new { Data = attempts, TotalRecords = totalRecords, TotalPages = totalPages, CurrentPage = page });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAttemptsForUser (Admin): {ex.ToString()}");
                return StatusCode(500, new { message = "Error fetching attempts for user." });
            }
        }

        [HttpGet("attempts/{attemptId}")] // GET api/v1/admin/lession-results/attempts/123
        public async Task<IActionResult> GetSpecificAttemptDetail(long attemptId)
        {
            try
            {
                var attemptDetail = await _resultService.GetLessionAttemptDetailByAdminAsync(attemptId);
                if (attemptDetail == null) return NotFound(new { message = "Attempt not found."});
                return Ok(attemptDetail);
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error in GetSpecificAttemptDetail (Admin): {ex.ToString()}");
                return StatusCode(500, new { message = "Error fetching specific attempt details." });
            }
        }
    }
}