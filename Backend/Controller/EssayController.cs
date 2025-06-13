// using Microsoft.AspNetCore.Mvc;
// using System.Threading.Tasks;
// using Backend.Data;
// using Backend.Models;
// using Backend.Service;

// namespace Backend.Controller
// {
//     [ApiController]
//     [Route("api/v1/correct")]
//     public class EssayController : ControllerBase
//     {
//         private readonly IDeepseekService _deepseekService;
//         private readonly AppDbContext _dbContext;

//         public EssayController(IDeepseekService deepseekService, AppDbContext dbContext)
//         {
//             _deepseekService = deepseekService;
//             _dbContext = dbContext;
//         }

//         [HttpPost]
//         public async Task<IActionResult> CorrectEssay([FromBody] string essayText)
//         {
//             var correctedText = await _deepseekService.CorrectEssayAsync(essayText);

//             var essay = new Essay
//             {
//                 OriginalText = essayText,
//                 CorrectedText = correctedText,
//                 CreatedAt = DateTime.UtcNow
//             };

//             _dbContext.Essays.Add(essay);
//             await _dbContext.SaveChangesAsync();

//             return Ok(new { correctedText });
//         }
//     }
// }