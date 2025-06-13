using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Dtos;
using Backend.Service;
using System.Threading.Tasks;
using System.Security.Claims;
using Backend.Exceptions; // Cho DataNotFoundException, InvalidOperationException
using System;              // Cho Exception
using System.Linq;         // Cho FirstOrDefault
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Controller
{
    [Route("api/v1/lession-results")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập cho tất cả các API này
    public class LessionResultController : ControllerBase
    {
        private readonly ILessionResultService _resultService;

        public LessionResultController(ILessionResultService resultService)
        {
            _resultService = resultService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out long userId))
            {
                throw new UnauthorizedAccessException("User ID not found or invalid in token.");
            }
            return userId;
        }

        [HttpPost("start")] // POST /api/v1/lession-results/start
        public async Task<IActionResult> StartAttempt([FromBody] StartLessionAttemptDto startDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                long userId = GetCurrentUserId();
                var attemptResult = await _resultService.StartLessionAttemptAsync(userId, startDto.lession_id);
                return Ok(attemptResult);
            }
            catch (DataNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new {message = ex.Message});}
            catch (Exception ex)
            {
                Console.WriteLine($"Error in StartAttempt: {ex.ToString()}");
                return StatusCode(500, new { message = "Error starting lession attempt." });
            }
        }

        [HttpPost("submit")] // POST /api/v1/lession-results/submit
        public async Task<IActionResult> SubmitAttempt([FromBody] SubmitLessionAttemptDto submissionDto)
        {
            if (!ModelState.IsValid || submissionDto.answers == null) return BadRequest(new { message = "Invalid submission data."});
            try
            {
                long userId = GetCurrentUserId();
                var finalResult = await _resultService.SubmitLessionAttemptAsync(userId, submissionDto);
                return Ok(finalResult);
            }
            catch (DataNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (InvalidParamException ex) { return BadRequest(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new {message = ex.Message});}
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SubmitAttempt: {ex.ToString()}");
                return StatusCode(500, new { message = "Error submitting lession attempt." });
            }
        }

        [HttpGet("history")] // GET /api/v1/lession-results/history?page=1&limit=10
        public async Task<IActionResult> GetMyHistory([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;
            try
            {
                long userId = GetCurrentUserId();
                var (history, totalRecords, totalPages) = await _resultService.GetUserLessionHistoryAsync(userId, page, limit);
                return Ok(new { Data = history, TotalRecords = totalRecords, TotalPages = totalPages, CurrentPage = page });
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new {message = ex.Message});}
            catch (Exception ex)
            {
                 Console.WriteLine($"Error in GetMyHistory: {ex.ToString()}");
                return StatusCode(500, new { message = "Error fetching user history." });
            }
        }

        [HttpGet("attempts/{attemptId}")] // GET /api/v1/lession-results/attempts/123
        public async Task<IActionResult> GetAttemptDetails(long attemptId)
        {
            try
            {
                long userId = GetCurrentUserId();
                var attemptDetail = await _resultService.GetLessionAttemptDetailForUserAsync(userId, attemptId);
                if (attemptDetail == null) return NotFound(new { message = "Attempt not found or access denied."});
                return Ok(attemptDetail);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new {message = ex.Message});}
            catch (Exception ex)
            {
                 Console.WriteLine($"Error in GetAttemptDetails: {ex.ToString()}");
                return StatusCode(500, new { message = "Error fetching attempt details." });
            }
        }
    }
}