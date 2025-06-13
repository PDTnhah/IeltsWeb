using Microsoft.AspNetCore.Mvc;
using Backend.Service;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Backend.Responses; // Cho PaginatedLessionResponse
using Backend.Dtos; // Cho LessionDetailViewDto

namespace Backend.Controller // Namespace cũ của bạn
{
    [ApiController]
    [Route("api/v1/lessions")]
    public class LessionController : ControllerBase
    {
        private readonly ILessionService _lessionService;

        public LessionController(ILessionService lessionService)
        {
            _lessionService = lessionService;
        }

        [HttpGet]
        // [AllowAnonymous] // Tùy theo yêu cầu login
        // [Authorize(Roles = "User,Admin")] // Ví dụ: Cả user và admin đều có thể xem (admin xem với quyền user)
        public async Task<IActionResult> GetLessionsForUser([FromQuery] long? skillId, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;
            // Logic is_published (nếu có) sẽ được xử lý trong LessionService khi gọi GetLessionsPaginatedAsync
            // Hiện tại, do không có is_published, nó sẽ trả về tất cả.
            var result = await _lessionService.GetLessionsPaginatedAsync(skillId, page, limit);
            return Ok(result);
        }

        [HttpGet("{id}")]
        // [AllowAnonymous]
        // [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetLessionDetailForUser(long id)
        {
            var lession = await _lessionService.GetLessionDetailByIdAsync(id); // Dùng hàm chung
            // Logic is_published (nếu có) sẽ được xử lý trong service
            if (lession == null)
            {
                return NotFound(new { message = "Lesson not found or not available." });
            }
            return Ok(lession);
        }
    }
}