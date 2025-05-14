using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Responses
{
    public class LessionResponse : BaseResponse
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(350, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 350 characters")]
        public string name { get; set; }

        [StringLength(300)]
        public string thumbnail { get; set; }

        public string description { get; set; }

        [JsonPropertyName("skill_id")]
        public long skillId { get; set; }
    }
}