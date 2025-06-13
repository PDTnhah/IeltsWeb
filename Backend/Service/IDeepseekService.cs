using Backend.Dtos;
using System.Threading.Tasks;

namespace Backend.Service
{
    public interface IDeepseekService
    {
        Task<AiFeedbackResponseDto> GetFeedbackAsync(AiFeedbackRequestDto requestDto);
    }
}