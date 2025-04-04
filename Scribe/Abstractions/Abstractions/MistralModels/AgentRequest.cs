using System.Text.Json.Serialization;

namespace Application.Abstractions.MistralModels
{

    public class AgentRequest
    {
        [JsonPropertyName("agent_id")]
        public string Agent { get; set; } = null!;

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new();
    }
}
