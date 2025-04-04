using System.Text.Json.Serialization;

namespace ParsingScenario.Abstractions.SOVAModels
{
    public class GenerationRequestModel
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = null!;
        [JsonPropertyName("voice")]
        public string Voice { get; set; } = "Natasha";
    }
}
