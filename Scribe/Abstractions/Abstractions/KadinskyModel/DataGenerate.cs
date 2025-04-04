using System.Text.Json.Serialization;

namespace Application.Abstractions.KadinskyModel
{
    internal class DataGenerate
    {
        [JsonPropertyName("model_id")]
        public string ModelId { get; set; } = null!;

        [JsonPropertyName("params")]
        public ParamsImage Params { get; set; } = null!;
    }
}
