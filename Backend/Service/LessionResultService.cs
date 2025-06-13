using AutoMapper;
using Backend.Data;
using Backend.Models;
using Backend.Dtos;
using Backend.Repositories;
using Backend.Exceptions; // Cho DataNotFoundException
using Microsoft.EntityFrameworkCore; // Cho transaction
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Service
{
    public class LessionResultService : ILessionResultService
    {
        private readonly ILessionAttemptRepository _attemptRepository;
        private readonly IAnswerAttemptRepository _answerRepository;
        private readonly ILessionRepository _lessionRepository;
        private readonly IQuestionRepository _questionRepository;
        // private readonly IChoiceRepository _choiceRepository; // Không cần trực tiếp nếu QuestionRepository đã load Choices
        private readonly IUserRepository _userRepository; // Để kiểm tra User tồn tại
        private readonly IMapper _mapper;
        private readonly AppDbContext _context; // Để dùng transaction

        public LessionResultService(
            ILessionAttemptRepository attemptRepository,
            IAnswerAttemptRepository answerRepository,
            ILessionRepository lessionRepository,
            IQuestionRepository questionRepository,
            // IChoiceRepository choiceRepository,
            IUserRepository userRepository,
            IMapper mapper,
            AppDbContext context)
        {
            _attemptRepository = attemptRepository;
            _answerRepository = answerRepository;
            _lessionRepository = lessionRepository;
            _questionRepository = questionRepository;
            // _choiceRepository = choiceRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<LessionAttemptResultDto> StartLessionAttemptAsync(long userId, long lessionId)
        {
            if (!await _userRepository.ExistsAsync(userId)) // Giả sử có hàm này trong IUserRepository
            {
                throw new DataNotFoundException($"User with ID {userId} not found.");
            }
            var lession = await _lessionRepository.GetByIdWithDetailsAsync(lessionId); // Lấy Lession cùng Questions
            if (lession == null)
            {
                throw new DataNotFoundException($"Lession with ID {lessionId} not found.");
            }

            var newAttempt = new LessionAttempt
            {
                user_id = userId,
                lession_id = lessionId,
                start_time = DateTime.UtcNow,
                total_questions = lession.Questions?.Count ?? 0,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };

            var createdAttemptEntity = await _attemptRepository.AddAsync(newAttempt);
            await _context.SaveChangesAsync(); // Lưu để có ID

            // Trả về thông tin attempt cùng với câu hỏi để user bắt đầu làm
            var resultDto = _mapper.Map<LessionAttemptResultDto>(createdAttemptEntity);
            resultDto.lession_name = lession.name;
            // Nếu LessionAttemptResultDto cần danh sách câu hỏi, bạn cần map chúng ở đây
            // Hoặc frontend sẽ gọi API lấy chi tiết Lession (đã có câu hỏi) riêng sau khi StartAttempt
            // Để đơn giản, ta có thể giả định frontend đã có LessionDetailViewDto từ trước
            // và chỉ cần LessionAttempt id từ response này.

            // Tạo một LessionAttemptResultDto cơ bản sau khi start
            return new LessionAttemptResultDto
            {
                id = createdAttemptEntity.id,
                lession_id = createdAttemptEntity.lession_id,
                lession_name = lession.name,
                start_time = createdAttemptEntity.start_time,
                total_questions = createdAttemptEntity.total_questions,
                // Các trường khác sẽ là null/default
                answer_details = new List<AnswerAttemptDetailDto>() // Danh sách câu trả lời rỗng
            };
        }

        public async Task<LessionAttemptResultDto> SubmitLessionAttemptAsync(long userId, SubmitLessionAttemptDto submissionDto)
        {
            var attempt = await _attemptRepository.GetByIdWithDetailsAsync(submissionDto.lession_attempt_id);
            if (attempt == null || attempt.user_id != userId)
            {
                throw new DataNotFoundException("Lession attempt not found or access denied.");
            }
            if (attempt.completed_at != null)
            {
                throw new InvalidOperationException("This lession attempt has already been submitted.");
            }

            int correctAnswersCount = 0;
            var answerDetailsForResponse = new List<AnswerAttemptDetailDto>();
            var answerEntitiesToSave = new List<AnswerAttempt>();

            if (submissionDto.answers == null || !submissionDto.answers.Any()) {
                 throw new InvalidParamException("No answers submitted.");
            }
            
            var lessionForDetails = await _lessionRepository.GetByIdWithDetailsAsync(attempt.lession_id);
            if (lessionForDetails == null) {
                // Lỗi nghiêm trọng nếu Lession của Attempt không tồn tại
                throw new DataNotFoundException($"Lession with ID {attempt.lession_id} associated with this attempt was not found.");
            }

            foreach (var submittedAnswerDto in submissionDto.answers)
            {
                var question = await _questionRepository.GetByIdAsync(submittedAnswerDto.question_id); // Lấy Question và Choices đúng
                if (question == null || question.lession_id != attempt.lession_id)
                {
                    Console.WriteLine($"Warning: Question ID {submittedAnswerDto.question_id} not found or does not belong to lession {attempt.lession_id}. Skipping.");
                    continue; // Bỏ qua nếu câu hỏi không tồn tại hoặc không thuộc bài học
                }

                bool currentAnswerIsCorrect = false;
                string? correctChoiceTextForDisplay = null;

                                switch (question.question_type)
                {
                    case QuestionType.MultipleChoice:
                        var correctMcChoice = question.Choices?.FirstOrDefault(c => c.is_correct);
                        if (submittedAnswerDto.selected_choice_id.HasValue && correctMcChoice != null &&
                            submittedAnswerDto.selected_choice_id.Value == correctMcChoice.id)
                        {
                            currentAnswerIsCorrect = true;
                        }
                        correctChoiceTextForDisplay = correctMcChoice?.choice_text;
                        break;

                    case QuestionType.TrueFalse:
                        // Giả định: Có 2 Choices, một là "True" (hoặc "1", "Đúng"), một là "False" (hoặc "0", "Sai")
                        // Và một trong số đó có is_correct = true.
                        // User sẽ gửi selected_choice_id tương ứng.
                        var correctTfChoice = question.Choices?.FirstOrDefault(c => c.is_correct);
                        if (submittedAnswerDto.selected_choice_id.HasValue && correctTfChoice != null &&
                            submittedAnswerDto.selected_choice_id.Value == correctTfChoice.id)
                        {
                            currentAnswerIsCorrect = true;
                        }
                        correctChoiceTextForDisplay = correctTfChoice?.choice_text;
                        break;

                    case QuestionType.FillInTheBlank:
                        // Giả định: Đáp án đúng được lưu trong question.explanation (có thể có nhiều đáp án cách nhau bởi '|')
                        // User gửi câu trả lời trong submittedAnswerDto.user_answer_text
                        if (!string.IsNullOrWhiteSpace(question.explanation) && !string.IsNullOrWhiteSpace(submittedAnswerDto.user_answer_text))
                        {
                            var possibleAnswers = question.explanation.Split('|').Select(ans => ans.Trim().ToLowerInvariant());
                            if (possibleAnswers.Contains(submittedAnswerDto.user_answer_text.Trim().ToLowerInvariant()))
                            {
                                currentAnswerIsCorrect = true;
                            }
                        }
                        correctChoiceTextForDisplay = question.explanation; // Hiển thị đáp án đúng là explanation
                        break;
                    default:
                        Console.WriteLine($"Warning: Unknown question type or scoring not implemented for type {question.question_type} (Question ID: {question.id}).");
                        break;
                }


                if (currentAnswerIsCorrect) correctAnswersCount++;

                answerEntitiesToSave.Add(new AnswerAttempt
                {
                    lession_attempt_id = attempt.id,
                    question_id = question.id,
                    selected_choice_id = submittedAnswerDto.selected_choice_id,
                    user_answer_text = submittedAnswerDto.user_answer_text,
                    is_correct = currentAnswerIsCorrect,
                    submitted_at = DateTime.UtcNow
                });

                answerDetailsForResponse.Add(new AnswerAttemptDetailDto
                {
                    question_id = question.id,
                    question_text = question.question_text,
                    question_type = question.question_type,
                    selected_choice_id = submittedAnswerDto.selected_choice_id,
                    user_answer_text = submittedAnswerDto.user_answer_text,
                    is_correct = currentAnswerIsCorrect,
                    correct_choice_text_if_any = correctChoiceTextForDisplay, // Lấy text của đáp án đúng
                    explanation = question.explanation
                });
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (answerEntitiesToSave.Any())
                    {
                        await _answerRepository.AddRangeAsync(answerEntitiesToSave); // Thêm tất cả câu trả lời
                    }

                    attempt.correct_answers = correctAnswersCount;
                    // total_questions có thể đã được set khi StartAttempt, hoặc lấy lại từ Lession
                    // var lessionForTotalQuestions = await _lessionRepository.GetByIdWithDetailsAsync(attempt.lession_id);
                    attempt.total_questions = lessionForDetails?.Questions?.Count;

                    attempt.score = (attempt.total_questions > 0)
                        ? Math.Round(((decimal)correctAnswersCount / attempt.total_questions.Value) * 100, 2)
                        : 0;
                    attempt.end_time = DateTime.UtcNow;
                    attempt.completed_at = DateTime.UtcNow;

                    await _attemptRepository.UpdateAsync(attempt); // Đánh dấu thay đổi
                    await _context.SaveChangesAsync(); // Lưu tất cả thay đổi (AnswerAttempts và LessionAttempt)
                    await transaction.CommitAsync();
                }
                catch(Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error during SubmitLessionAttempt transaction: {ex.ToString()}");
                    throw new Exception("Error submitting lession attempt: " + ex.Message, ex);
                }
            }

            // Tạo DTO kết quả cuối cùng
            var finalResultDto = _mapper.Map<LessionAttemptResultDto>(attempt);
            finalResultDto.lession_name = attempt.Lession?.name ?? lessionForDetails?.name ?? "N/A";
            finalResultDto.answer_details = answerDetailsForResponse;
            return finalResultDto;
        }

        public async Task<(IEnumerable<UserLessionHistoryDto> History, int TotalRecords, int TotalPages)> GetUserLessionHistoryAsync(long userId, int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            var attempts = await _attemptRepository.GetByUserIdAsync(userId, page, limit);
            var totalRecords = await _attemptRepository.CountByUserIdAsync(userId);
            var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

            return (_mapper.Map<IEnumerable<UserLessionHistoryDto>>(attempts), totalRecords, totalPages);
        }

        public async Task<LessionAttemptResultDto?> GetLessionAttemptDetailForUserAsync(long userId, long attemptId)
        {
            var attempt = await _attemptRepository.GetByIdWithDetailsAsync(attemptId);
            if (attempt == null || attempt.user_id != userId)
            {
                return null; // Hoặc throw UnauthorizedAccessException
            }
            // Map chi tiết câu trả lời
            var resultDto = _mapper.Map<LessionAttemptResultDto>(attempt);
            // `answer_details` đã được map từ `attempt.AnswerAttempts` nhờ AutoMapper
            return resultDto;
        }


        // --- Các hàm cho Admin ---
        public async Task<(IEnumerable<LessionAttemptResultDto> Attempts, int TotalRecords, int TotalPages)> GetAttemptsForLessionByAdminAsync(long lessionId, int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            var attempts = await _attemptRepository.GetByLessionIdAsync(lessionId, page, limit);
            var totalRecords = await _attemptRepository.CountByLessionIdAsync(lessionId);
            var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

            return (_mapper.Map<IEnumerable<LessionAttemptResultDto>>(attempts), totalRecords, totalPages);
        }

        public async Task<(IEnumerable<LessionAttemptResultDto> Attempts, int TotalRecords, int TotalPages)> GetAttemptsForUserByAdminAsync(long userId, int page, int limit)
        {
             // Tương tự GetUserLessionHistoryAsync nhưng trả về LessionAttemptResultDto
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            var attempts = await _attemptRepository.GetByUserIdAsync(userId, page, limit); // Dùng lại hàm này
            var totalRecords = await _attemptRepository.CountByUserIdAsync(userId);
            var totalPages = (int)Math.Ceiling((double)totalRecords / limit);
            // Cần đảm bảo GetByUserIdAsync load đủ thông tin để map sang LessionAttemptResultDto
            // Hoặc tạo một hàm GetByUserIdWithDetailsAsync trong repo
            var detailedAttempts = new List<LessionAttemptResultDto>();
            foreach(var attempt in attempts)
            {
                var detail = await GetLessionAttemptDetailByAdminAsync(attempt.id); // Lấy chi tiết từng attempt
                if(detail != null) detailedAttempts.Add(detail);
            }
            return (detailedAttempts, totalRecords, totalPages);
        }

        public async Task<LessionAttemptResultDto?> GetLessionAttemptDetailByAdminAsync(long attemptId)
        {
            var attempt = await _attemptRepository.GetByIdWithDetailsAsync(attemptId);
            // Admin có thể xem bất kỳ attempt nào
            return _mapper.Map<LessionAttemptResultDto>(attempt);
        }
    }
}