using System.Text.Json.Serialization;

namespace Application.Abstractions.KadinskyModel
{
    public class GetModels
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("version")]
        public float Version { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;
    }
}
