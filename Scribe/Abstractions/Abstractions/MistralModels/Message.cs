using System.Text.Json.Serialization;

namespace Application.Abstractions.MistralModels
{
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;

        [JsonPropertyName("content")]
        public string Content { get; set; } = null!;
    }
}
