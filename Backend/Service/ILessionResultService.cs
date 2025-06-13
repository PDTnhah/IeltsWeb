using Backend.Dtos; // Chứa các DTO liên quan đến kết quả
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Service
{
    public interface ILessionResultService
    {
        // Bắt đầu một lần làm bài mới cho User hiện tại
        Task<LessionAttemptResultDto> StartLessionAttemptAsync(long userId, long lessionId);

        // User nộp bài làm
        Task<LessionAttemptResultDto> SubmitLessionAttemptAsync(long userId, SubmitLessionAttemptDto submissionDto);

        // User xem lịch sử các lần làm bài của mình (phân trang)
        Task<(IEnumerable<UserLessionHistoryDto> History, int TotalRecords, int TotalPages)> GetUserLessionHistoryAsync(long userId, int page, int limit);

        // User xem chi tiết một lần làm bài cụ thể của mình
        Task<LessionAttemptResultDto?> GetLessionAttemptDetailForUserAsync(long userId, long attemptId);

        // --- Các hàm cho Admin ---
        // Admin xem tất cả các lần làm bài của một Lession (phân trang)
        Task<(IEnumerable<LessionAttemptResultDto> Attempts, int TotalRecords, int TotalPages)> GetAttemptsForLessionByAdminAsync(long lessionId, int page, int limit);

        // Admin xem tất cả các lần làm bài của một User (phân trang)
        Task<(IEnumerable<LessionAttemptResultDto> Attempts, int TotalRecords, int TotalPages)> GetAttemptsForUserByAdminAsync(long userId, int page, int limit);

        // Admin xem chi tiết một lần làm bài bất kỳ
        Task<LessionAttemptResultDto?> GetLessionAttemptDetailByAdminAsync(long attemptId);
    }
}