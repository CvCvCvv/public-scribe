using System.Text.Json.Serialization;

namespace Application.Abstractions.KadinskyModel
{
    public class Generation
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("images")]
        public List<string> Images { get; set; } = new();

        [JsonPropertyName("errorDescription")]
        public string ErrorDescription { get; set; } = null!;

        [JsonPropertyName("censored")]
        public bool Censored { get; set; }
    }
}