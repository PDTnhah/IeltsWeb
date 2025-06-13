// Backend.Service/ILessionService.cs
using Backend.Dtos;      // Namespace chứa LessionDtos, QuestionInputDto, LessionDetailViewDto etc.
using Backend.Responses; // Namespace chứa PaginatedLessionResponse
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Service
{
    public interface ILessionService
    {
        // Sử dụng LessionDtos (tên cũ của bạn) làm DTO đầu vào
        Task<LessionDetailViewDto> CreateLessionWithQuestionsAsync(LessionDtos lessionInputDto);
        Task<LessionDetailViewDto?> UpdateLessionWithQuestionsAsync(long lessionId, LessionDtos lessionInputDto);
        Task<bool> DeleteLessionAsync(long lessionId);

        Task<LessionDetailViewDto?> GetLessionDetailByIdAsync(long lessionId); // Gộp hàm lấy chi tiết

        Task<PaginatedLessionResponse> GetLessionsPaginatedAsync(long? skillId, int page, int limit);
        Task<int> GetLessionTotalCountAsync(long? skillId);

        Task<bool> LessionExistsAsync(long lessionId);
        Task<bool> LessionExistsByNameAsync(string name, long? currentIdToExclude = null);

        // Bỏ LessionImage nếu không dùng
        // LessionImage CreateLessionImage(long id, LessionImageDto lessionImageDto);
    }
}