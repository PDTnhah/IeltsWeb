// LessionResultDtos.cs (Mới, trong Backend.Dtos)
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend.Models; // Cho QuestionType

namespace Backend.Dtos
{
    // DTO người dùng gửi lên khi bắt đầu làm bài
    public class StartLessionAttemptDto
    {
        [Required]
        public long lession_id { get; set; }
        // public long user_id; // Sẽ lấy từ token
    }

    // DTO người dùng gửi lên cho mỗi câu trả lời (có thể gửi một mảng khi nộp bài)
    public class SubmitAnswerDto
    {
        [Required]
        public long question_id { get; set; }
        public long? selected_choice_id { get; set; } // Cho trắc nghiệm
        public string? user_answer_text { get; set; } // Cho các loại khác
    }

    // DTO người dùng gửi lên khi nộp toàn bộ bài
    public class SubmitLessionAttemptDto
    {
        [Required]
        public long lession_attempt_id { get; set; } // ID của lần làm bài
        public List<SubmitAnswerDto> answers { get; set; } = new List<SubmitAnswerDto>();
    }

    // DTO trả về kết quả chi tiết của một lần làm bài
    public class LessionAttemptResultDto
    {
        public long id { get; set; }
        public long lession_id { get; set; }
        public string lession_name { get; set; } // Tên bài học
        public decimal? score { get; set; }
        public int? total_questions { get; set; }
        public int? correct_answers { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public DateTime? completed_at { get; set; }
        public List<AnswerAttemptDetailDto> answer_details { get; set; }
    }

    public class AnswerAttemptDetailDto
    {
        public long question_id { get; set; }
        public string question_text { get; set; }
        public QuestionType question_type { get; set; }
        public long? selected_choice_id { get; set; }
        public string? user_answer_text { get; set; }
        public bool? is_correct { get; set; }
        public string? correct_choice_text_if_any { get; set; } // Đáp án đúng của câu trắc nghiệm
        public string? explanation {get; set;} // Giải thích câu hỏi
    }

    // DTO cho lịch sử làm bài của người dùng
    public class UserLessionHistoryDto
    {
        public long attempt_id { get; set; }
        public long lession_id { get; set; }
        public string lession_name { get; set; }
        public decimal? score { get; set; }
        public DateTime completed_at { get; set; }
    }
}