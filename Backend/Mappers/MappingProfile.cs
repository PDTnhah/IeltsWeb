using AutoMapper;
using Backend.Models;
using Backend.Dtos;
using Backend.Responses; // Cho LessionListResponseItemDto, PaginatedLessionResponse nếu chúng ở namespace này
using System.Linq;    // Cho .FirstOrDefault() và .Select()

namespace Backend.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Lession Mappings ---
            // Từ DTO (đầu vào khi Admin tạo/sửa) sang Model Lession
            CreateMap<LessionDtos, Lession>() // LessionDtos là DTO bạn cung cấp
                .ForMember(dest => dest.skillId, opt => opt.MapFrom(src => src.skillId)) // Map từ skillId (DTO) sang skillId (Model)
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.questions)) // Map danh sách QuestionInputDto sang ICollection<Question>
                .ForMember(dest => dest.skill, opt => opt.Ignore()) // Không map skill object khi tạo/update từ DTO, chỉ dùng skillId
                .ForMember(dest => dest.id, opt => opt.Ignore()) // Không map id khi tạo mới từ DTO
                .ForMember(dest => dest.createdAt, opt => opt.Ignore()) // Để BaseEntity hoặc Service xử lý
                .ForMember(dest => dest.updatedAt, opt => opt.Ignore()); // Để BaseEntity hoặc Service xử lý
                // Các trường name, thumbnail, description, main_content, audio_url, transcript sẽ được AutoMapper tự map do tên giống nhau.
                // is_published đã bị bỏ.

            // Từ Model Lession sang LessionListResponseItemDto (dùng cho danh sách hiển thị)
            CreateMap<Lession, LessionListResponseItemDto>()
                .ForMember(dest => dest.skillName, opt => opt.MapFrom(src => src.skill != null ? src.skill.name : null))
                .ForMember(dest => dest.questionCount, opt => opt.MapFrom(src => src.Questions != null ? src.Questions.Count : 0));
                // is_published đã bị bỏ.

            // Từ Model Lession sang LessionDetailViewDto (dùng cho trang xem chi tiết bài học)
            CreateMap<Lession, LessionDetailViewDto>()
                .ForMember(dest => dest.skillName, opt => opt.MapFrom(src => src.skill != null ? src.skill.name : null))
                .ForMember(dest => dest.questions, opt => opt.MapFrom(src => src.Questions)); // Map ICollection<Question> sang List<QuestionResponseDto>
                // is_published đã bị bỏ.

            // --- Question Mappings ---
            // Từ QuestionInputDto (đầu vào khi Admin tạo/sửa Question) sang Model Question
            CreateMap<QuestionInputDto, Question>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.choices)) // Map List<ChoiceInputDto> sang ICollection<Choice>
                .ForMember(dest => dest.id, opt => opt.Ignore()) // Không map id khi tạo mới
                .ForMember(dest => dest.lession_id, opt => opt.Ignore()) // lession_id sẽ được gán ở Service
                .ForMember(dest => dest.Lession, opt => opt.Ignore()) // Không map Lession object
                .ForMember(dest => dest.createdAt, opt => opt.Ignore())
                .ForMember(dest => dest.updatedAt, opt => opt.Ignore());

            // Từ Model Question sang QuestionResponseDto (để trả về cho client)
            CreateMap<Question, QuestionResponseDto>()
                .ForMember(dest => dest.choices, opt => opt.MapFrom(src => src.Choices)); // Map ICollection<Choice> sang List<ChoiceResponseDto>

            // --- Choice Mappings ---
            // Từ ChoiceInputDto (đầu vào khi Admin tạo/sửa Choice) sang Model Choice
            CreateMap<ChoiceInputDto, Choice>()
                .ForMember(dest => dest.id, opt => opt.Ignore()) // Không map id khi tạo mới
                .ForMember(dest => dest.question_id, opt => opt.Ignore()) // question_id sẽ được gán ở Service
                .ForMember(dest => dest.Question, opt => opt.Ignore()) // Không map Question object
                .ForMember(dest => dest.createdAt, opt => opt.Ignore())
                .ForMember(dest => dest.updatedAt, opt => opt.Ignore());
            // Từ Model Choice sang ChoiceResponseDto (để trả về cho client)
            CreateMap<Choice, ChoiceResponseDto>();


            // --- LessionAttempt Mappings ---
            // Từ Model LessionAttempt sang LessionAttemptResultDto (kết quả chi tiết một lần làm bài)
            CreateMap<LessionAttempt, LessionAttemptResultDto>()
                .ForMember(dest => dest.lession_name, opt => opt.MapFrom(src => src.Lession != null ? src.Lession.name : "N/A"))
                .ForMember(dest => dest.answer_details, opt => opt.MapFrom(src => src.AnswerAttempts)); // Map ICollection<AnswerAttempt> sang List<AnswerAttemptDetailDto>

            // Từ Model LessionAttempt sang UserLessionHistoryDto (cho lịch sử làm bài của User)
            CreateMap<LessionAttempt, UserLessionHistoryDto>()
                .ForMember(dest => dest.attempt_id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.lession_name, opt => opt.MapFrom(src => src.Lession != null ? src.Lession.name : "N/A"))
                .ForMember(dest => dest.completed_at, opt => opt.MapFrom(src => src.completed_at)); // Lấy completed_at


            // --- AnswerAttempt Mappings ---
            // Từ Model AnswerAttempt sang AnswerAttemptDetailDto (chi tiết một câu trả lời)
            CreateMap<AnswerAttempt, AnswerAttemptDetailDto>()
                .ForMember(dest => dest.question_id, opt => opt.MapFrom(src => src.question_id)) // Đã có sẵn
                .ForMember(dest => dest.question_text, opt => opt.MapFrom(src => src.Question != null ? src.Question.question_text : null))
                .ForMember(dest => dest.question_type, opt => opt.MapFrom(src => src.Question != null ? src.Question.question_type : default(QuestionType)))
                .ForMember(dest => dest.selected_choice_id, opt => opt.MapFrom(src => src.selected_choice_id))
                .ForMember(dest => dest.user_answer_text, opt => opt.MapFrom(src => src.user_answer_text))
                .ForMember(dest => dest.is_correct, opt => opt.MapFrom(src => src.is_correct))
                .ForMember(dest => dest.explanation, opt => opt.MapFrom(src => src.Question != null ? src.Question.explanation : null))
                .ForMember(dest => dest.correct_choice_text_if_any, opt => opt.MapFrom(src =>
                    // Kiểm tra null cho src.Question và src.Question.Choices trước khi truy cập
                    (src.Question != null && src.Question.Choices != null &&
                     (src.Question.question_type == QuestionType.MultipleChoice || src.Question.question_type == QuestionType.TrueFalse))
                        ? (src.Question.Choices.FirstOrDefault(c => c.is_correct) != null ? src.Question.Choices.FirstOrDefault(c => c.is_correct).choice_text : null) // An toàn hơn với ?.
                        : (src.Question != null && src.Question.question_type == QuestionType.FillInTheBlank)
                            ? src.Question.explanation // Cho FillInTheBlank, đáp án mẫu là explanation
                            : null
                ));
        }
    }
}