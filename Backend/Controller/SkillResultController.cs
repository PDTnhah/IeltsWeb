using Microsoft.AspNetCore.Mvc;
using System;

namespace Backend.Controller
{
    [ApiController]
    [Route("api/v1/skill-result")]
    public class SkillResultController : ControllerBase
    {
        [HttpGet("{user_id}")]
        public IActionResult GetLessionResult(long userId)
        {
            try
            {
                // Chưa có logic đầy đủ, cần thêm service
                return Ok("Get result");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}