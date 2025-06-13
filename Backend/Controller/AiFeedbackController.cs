using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Dtos;
using Backend.Service;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Nếu muốn bảo vệ API này
using System;

namespace Backend.Controller
{
    [Route("api/v1/ai-feedback")]
    [ApiController]
    // [Authorize] // Yêu cầu người dùng đăng nhập để sử dụng tính năng này
    public class AiFeedbackController : ControllerBase
    {
        private readonly IDeepseekService _deepseekService;
        private readonly ILogger<AiFeedbackController> _logger; // Thêm Logger

        public AiFeedbackController(IDeepseekService deepseekService, ILogger<AiFeedbackController> logger)
        {
            _deepseekService = deepseekService;
            _logger = logger;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        // Sử dụng [FromForm] để nhận cả text và file
        public async Task<IActionResult> GetAiFeedback([FromForm] AiFeedbackRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("GetAiFeedback: Invalid ModelState: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            // Kiểm tra hoặc là TextContent hoặc AudioFile phải được cung cấp tùy theo FeedbackType
            if (requestDto.FeedbackType.Equals("writing", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(requestDto.TextContent))
            {
                return BadRequest(new { message = "TextContent is required for writing feedback." });
            }
            if (requestDto.FeedbackType.Equals("speaking", StringComparison.OrdinalIgnoreCase) && requestDto.AudioFile == null)
            {
                return BadRequest(new { message = "AudioFile is required for speaking feedback." });
            }


            try
            {
                _logger.LogInformation("Receiving AI Feedback request for type: {FeedbackType}", requestDto.FeedbackType);
                var feedbackResponse = await _deepseekService.GetFeedbackAsync(requestDto);

                if (feedbackResponse.Success)
                {
                    return Ok(feedbackResponse);
                }
                else
                {
                    _logger.LogWarning("AI Feedback generation failed: {ErrorMessage}", feedbackResponse.ErrorMessage);
                    // Trả về lỗi cụ thể hơn nếu có từ service
                    return BadRequest(new { message = feedbackResponse.ErrorMessage ?? "Failed to get AI feedback." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetAiFeedback for type: {FeedbackType}", requestDto.FeedbackType);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while processing your request." });
            }
        }
    }
}