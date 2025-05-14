using System;
using System.Text.Json.Serialization;

namespace Backend.Responses
{
    public abstract class BaseResponse
    {
        [JsonPropertyName("created_at")]
        public DateTime createdAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime updatedAt { get; set; }
    }
}