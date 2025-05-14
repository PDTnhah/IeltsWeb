using System;
using System.Text.Json.Serialization;

namespace Backend.Dtos
{
    public class SkillResultDto
    {
        [JsonPropertyName("user_id")]
        public long userId { get; set; }

        [JsonPropertyName("skill_id")]
        public long skillId { get; set; }

        public float score { get; set; }

        [JsonPropertyName("completed_at")]
        public DateTime completedAt { get; set; }
    }
}