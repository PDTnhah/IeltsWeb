using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.Dtos
{

    public class LessionDtos
    {
        [Required(ErrorMessage = "Ten bai hoc khong bo trong")]
        [StringLength(350, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string name { get; set; }

        public string? thumbnail { get; set; }

        public string description { get; set; }
        public string? main_content { get; set; }
        [Url(ErrorMessage = "Audio URL không hợp lệ")]
        public string? audio_url { get; set; }
        public string? transcript { get; set; }


        [JsonPropertyName("skill_id")]
        public long skillId { get; set; }

        public List<QuestionInputDto> questions { get; set; } = new List<QuestionInputDto>();

    }
}