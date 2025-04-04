using System.Text.Json.Serialization;

namespace Application.Abstractions.KadinskyModel
{
    public class GenerateParams
    {
        [JsonPropertyName("query")]
        public string Query { get; set; } = null!;
    }
}
