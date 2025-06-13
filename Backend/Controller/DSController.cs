    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Backend.Service; // Điều chỉnh namespace cho DeepSeekService

    namespace Backend.Controller
    {
        [ApiController]
        [Route("api/[controller]")]
        public class DeepSeekController : ControllerBase
        {
            private readonly NewDeepSeekService _deepSeekService;

            public DeepSeekController(NewDeepSeekService deepSeekService)
            {
                _deepSeekService = deepSeekService;
            }

            /// <summary>
            /// POST api/deepseek/analyze
            /// Nhận Ielts Writing hoặc ghi âm mp3, trả về nội dung đã được AI chỉnh sửa/đánh giá.
            /// Hiện tại chỉ hỗ trợ text; audio sẽ được mở rộng sau.
            /// </summary>
            /// <param name="request">Yêu cầu chứa trường Text hoặc Audio</param>
            /// <returns>Đoạn văn đã được chỉnh sửa</returns>
            [HttpPost("analyze")]
            public async Task<IActionResult> Analyze([FromForm] DeepSeekRequest request)
            {
                if (string.IsNullOrWhiteSpace(request.Text) && request.Audio == null)
                {
                    return BadRequest(new { error = "Vui lòng cung cấp text hoặc file audio mp3." });
                }

                // Xử lý text
                // if (!string.IsNullOrWhiteSpace(request.Text))
                // {
                    // string prompt = "Please correct and provide feedback for the following IELTS writing:\n" + request.Text;
                try {
                string content;
                if (!string.IsNullOrWhiteSpace(request.Text))
                {
                    content = $"Please correct and provide feedback for the following IELTS writing:\n{request.Text}";
                } else {
                    // Xử lý audio
                    await using var stream = request.Audio.OpenReadStream();
                    var transcript = await _deepSeekService.TranscribeAudioAsync(stream, request.Audio.FileName);
                    // Tạo prompt cho speaking
                    content = $"Please provide feedback and corrections for the following IELTS speaking transcript:\n{transcript}";
                }

                var result = await _deepSeekService.CallDeepSeekAsync(content);
                return Ok(new { result });
                } catch (Exception ex)
                    {
                        return StatusCode(500, new { error = ex.Message });
                    }
                

                // Xử lý audio (chưa triển khai)
                // return BadRequest(new { error = "Chức năng chuyển audio sang text chưa được hỗ trợ." });
            }
        }

        /// <summary>
        /// Model request cho DeepSeekController
        /// </summary>
        public class DeepSeekRequest
        {
            /// <summary>Đoạn văn IELTS để AI chỉnh sửa</summary>
            public string? Text { get; set; }

            /// <summary>File mp3 ghi âm Speaking</summary>
            public IFormFile? Audio { get; set; }
        }
    }
