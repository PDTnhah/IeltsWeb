using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Dtos
{
    public class LessionDtos
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string name { get; set; }

        public string? thumbnail { get; set; }

        public string description { get; set; }

        [JsonPropertyName("skill_id")]
        public long skillId { get; set; }
    }
}