// Backend.Service/LessionService.cs
using AutoMapper;
using Backend.Data;
using Backend.Models; // Đảm bảo using này để tham chiếu đến Model đúng
using Backend.Dtos;
using Backend.Responses;
using Backend.Repositories;
using Backend.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Service
{
    public class LessionService : ILessionService
    {
        private readonly ILessionRepository _lessionRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IChoiceRepository _choiceRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public LessionService(
            ILessionRepository lessionRepository,
            ISkillRepository skillRepository,
            IQuestionRepository questionRepository,
            IChoiceRepository choiceRepository,
            IMapper mapper,
            AppDbContext context)
        {
            _lessionRepository = lessionRepository;
            _skillRepository = skillRepository;
            _questionRepository = questionRepository;
            _choiceRepository = choiceRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<LessionDetailViewDto> CreateLessionWithQuestionsAsync(LessionDtos lessionInputDto)
        {
            if (await LessionExistsByNameAsync(lessionInputDto.name))
            {
                throw new InvalidParamException($"Lession with name '{lessionInputDto.name}' already exists.");
            }
            // Giả sử _skillRepository.FindById đã là async Task<Skill?>
            var skill = await _skillRepository.FindByIdAsync(lessionInputDto.skillId);
            if (skill == null)
            {
                throw new DataNotFoundException($"Skill with ID {lessionInputDto.skillId} not found.");
            }

            var lession = _mapper.Map<Lession>(lessionInputDto);
            lession.skillId = skill.id;
            lession.skill = null;
            lession.createdAt = DateTime.UtcNow; // Gán nếu Lession kế thừa BaseEntity hoặc có trường này
            lession.updatedAt = DateTime.UtcNow; // Gán nếu Lession kế thừa BaseEntity hoặc có trường này

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var createdLessionEntity = await _lessionRepository.SaveAsync(lession);
                    // Không cần SaveChangesAsync ở đây nếu SaveAsync của Repo đã làm
                    if (createdLessionEntity.id == 0) {
                        throw new Exception("Failed to create Lession or retrieve its ID.");
                    }

                    if (lessionInputDto.questions != null && lessionInputDto.questions.Any())
                    {
                        foreach (var qDto in lessionInputDto.questions)
                        {
                            var question = _mapper.Map<Question>(qDto); // Map sang Backend.Models.Question
                            question.lession_id = createdLessionEntity.id;
                            question.id = 0;
                            question.createdAt = DateTime.UtcNow; // Gán nếu Question có
                            question.updatedAt = DateTime.UtcNow; // Gán nếu Question có

                            _context.Questions.Add(question); // Chỉ thêm vào context
                            await _context.SaveChangesAsync(); // <<<< LƯU QUESTION VÀO DB ĐỂ LẤY ID THẬT
                                                       // createdQuestionEntity.id bây giờ sẽ có giá trị đúng
                            var createdQuestionEntity = question; // Entity 'question' giờ đã được EF Core theo dõi và có ID

                            if (createdQuestionEntity.id == 0)
                            {
                                throw new Exception($"Failed to create Question '{question.question_text}' or retrieve its ID.");
                            }

                            if (qDto.choices != null && qDto.choices.Any())
                            {
                                foreach (var cDto in qDto.choices)
                                {
                                    var choice = _mapper.Map<Choice>(cDto); // Map sang Backend.Models.Choice
                                    choice.question_id = createdQuestionEntity.id;
                                    choice.id = 0;
                                    choice.createdAt = DateTime.UtcNow; // Gán nếu Choice có
                                    choice.updatedAt = DateTime.UtcNow; // Gán nếu Choice có
                                    await _choiceRepository.AddAsync(choice);
                                }
                            }
                        }
                    }
                    await _context.SaveChangesAsync(); // Commit tất cả thay đổi trong transaction
                    await transaction.CommitAsync();

                    var resultLession = await _lessionRepository.GetByIdWithDetailsAsync(createdLessionEntity.id);
                    return _mapper.Map<LessionDetailViewDto>(resultLession);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error in CreateLessionWithQuestionsAsync: {ex.Message} - StackTrace: {ex.StackTrace}");
                    throw new Exception("Error creating lession with questions: " + ex.Message, ex);
                }
            }
        }

        public async Task<LessionDetailViewDto?> UpdateLessionWithQuestionsAsync(long lessionId, LessionDtos lessionInputDto)
        {
            var existingLession = await _lessionRepository.GetByIdWithDetailsAsync(lessionId);
            if (existingLession == null) return null;

            if (lessionInputDto.name != existingLession.name && await LessionExistsByNameAsync(lessionInputDto.name, lessionId))
            {
                 throw new InvalidParamException($"Another Lession with name '{lessionInputDto.name}' already exists.");
            }
            var skill = await _skillRepository.FindByIdAsync(lessionInputDto.skillId); // Giả sử async
            if (skill == null)
            {
                throw new DataNotFoundException($"Skill with ID {lessionInputDto.skillId} not found.");
            }

            // Map các trường cơ bản của Lession
            existingLession.name = lessionInputDto.name;
            existingLession.thumbnail = lessionInputDto.thumbnail;
            existingLession.description = lessionInputDto.description;
            existingLession.main_content = lessionInputDto.main_content;
            existingLession.audio_url = lessionInputDto.audio_url;
            existingLession.transcript = lessionInputDto.transcript;
            existingLession.skillId = lessionInputDto.skillId;
            existingLession.updatedAt = DateTime.UtcNow; // Gán nếu Lession có

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var questionsFromDto = lessionInputDto.questions ?? new List<QuestionInputDto>();
                    var existingQuestionIdsFromDto = questionsFromDto.Where(q => q.id.HasValue && q.id.Value != 0).Select(q => q.id.Value).ToList();
                    
                    var currentDbQuestions = existingLession.Questions.ToList(); // Load vào memory để thao tác
                    var questionsToDelete = currentDbQuestions
                        .Where(eq => !existingQuestionIdsFromDto.Contains(eq.id))
                        .ToList();

                    if (questionsToDelete.Any())
                    {
                        await _questionRepository.DeleteRangeAsync(questionsToDelete);
                    }

                    foreach (var qDto in questionsFromDto)
                    {
                        Question questionEntity; // Backend.Models.Question
                        if (qDto.id.HasValue && qDto.id.Value != 0) // Update existing Question
                        {
                            questionEntity = currentDbQuestions.FirstOrDefault(eq => eq.id == qDto.id.Value);
                            if (questionEntity != null)
                            {
                                _mapper.Map(qDto, questionEntity);
                                questionEntity.updatedAt = DateTime.UtcNow; // Gán nếu Question có
                                // Xử lý Choices
                                var choicesFromDto = qDto.choices ?? new List<ChoiceInputDto>();
                                var existingChoiceIdsFromDto = choicesFromDto.Where(c => c.id.HasValue && c.id.Value != 0).Select(c => c.id.Value).ToList();
                                
                                var currentDbChoices = questionEntity.Choices.ToList(); // Load vào memory
                                var choicesToDelete = currentDbChoices
                                    .Where(ec => !existingChoiceIdsFromDto.Contains(ec.id))
                                    .ToList();
                                if (choicesToDelete.Any())
                                {
                                    await _choiceRepository.DeleteRangeAsync(choicesToDelete);
                                }

                                foreach (var cDto in choicesFromDto)
                                {
                                    Choice choiceEntity; // Backend.Models.Choice
                                    if (cDto.id.HasValue && cDto.id.Value != 0) // Update existing Choice
                                    {
                                        choiceEntity = currentDbChoices.FirstOrDefault(ec => ec.id == cDto.id.Value);
                                        if (choiceEntity != null)
                                        {
                                            _mapper.Map(cDto, choiceEntity);
                                            choiceEntity.updatedAt = DateTime.UtcNow; // Gán nếu Choice có
                                        }
                                    }
                                    else // Add new Choice
                                    {
                                        choiceEntity = _mapper.Map<Choice>(cDto);
                                        choiceEntity.question_id = questionEntity.id;
                                        choiceEntity.id = 0;
                                        choiceEntity.createdAt = DateTime.UtcNow; // Gán nếu Choice có
                                        choiceEntity.updatedAt = DateTime.UtcNow; // Gán nếu Choice có
                                        // _context.Choices.Add(choiceEntity); // Hoặc dùng repo
                                        await _choiceRepository.AddAsync(choiceEntity);
                                    }
                                }
                            }
                        }
                        else // Add new Question
                        {
                            questionEntity = _mapper.Map<Question>(qDto);
                            questionEntity.lession_id = existingLession.id;
                            questionEntity.id = 0;
                            questionEntity.createdAt = DateTime.UtcNow; // Gán nếu Question có
                            questionEntity.updatedAt = DateTime.UtcNow; // Gán nếu Question có
                            // Add Choices cho Question mới
                            if (qDto.choices != null && qDto.choices.Any())
                            {
                                foreach(var cDto in qDto.choices)
                                {
                                    var newChoice = _mapper.Map<Choice>(cDto);
                                    newChoice.id = 0;
                                    newChoice.createdAt = DateTime.UtcNow;
                                    newChoice.updatedAt = DateTime.UtcNow;
                                    questionEntity.Choices.Add(newChoice); // Add vào collection của Question
                                }
                            }
                            existingLession.Questions.Add(questionEntity); // Add Question vào Lession
                        }
                    }
                    await _context.SaveChangesAsync(); // Lưu tất cả thay đổi
                    await transaction.CommitAsync();

                    var resultLession = await _lessionRepository.GetByIdWithDetailsAsync(lessionId);
                    return _mapper.Map<LessionDetailViewDto>(resultLession);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error in UpdateLessionWithQuestionsAsync: {ex.Message} - StackTrace: {ex.StackTrace}");
                    throw new Exception("Error updating lession: " + ex.Message, ex);
                }
            }
        }
        // ... (Các phương thức khác đã được cập nhật ở câu trả lời trước)
        public async Task<bool> DeleteLessionAsync(long id)
        {
            var lession = await _lessionRepository.FindByIdAsync(id);
            if (lession != null)
            {
                await _lessionRepository.DeleteAsync(lession);
                return true;
            }
            return false;
        }

        public async Task<LessionDetailViewDto?> GetLessionDetailByIdAsync(long lessionId)
        {
            var lession = await _lessionRepository.GetByIdWithDetailsAsync(lessionId);
            if (lession == null)
            {
                return null;
            }
            return _mapper.Map<LessionDetailViewDto>(lession);
        }

        public async Task<PaginatedLessionResponse> GetLessionsPaginatedAsync(long? skillId, int page, int limit)
        {
            var (lessions, totalRecords) = await _lessionRepository.FindAllPaginatedAsync(skillId, page, limit);
            var lessionDtos = _mapper.Map<List<LessionListResponseItemDto>>(lessions);
            return new PaginatedLessionResponse
            {
                lessions = lessionDtos,
                totalPages = (int)Math.Ceiling((double)totalRecords / limit),
                currentPage = page,
                totalRecords = totalRecords
            };
        }

        public async Task<int> GetLessionTotalCountAsync(long? skillId)
        {
            return await _lessionRepository.CountAsync(skillId);
        }

        public async Task<bool> LessionExistsAsync(long lessionId)
        {
            return await _lessionRepository.ExistsAsync(lessionId);
        }

        public async Task<bool> LessionExistsByNameAsync(string name, long? currentIdToExclude = null)
        {
            return await _lessionRepository.ExistsByNameAsync(name, currentIdToExclude);
        }
    }
}