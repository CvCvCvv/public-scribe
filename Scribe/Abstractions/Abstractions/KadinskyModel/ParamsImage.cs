using System.Text.Json.Serialization;

namespace Application.Abstractions.KadinskyModel
{
    public class ParamsImage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "GENERATE";

        [JsonPropertyName("numImages")]
        public int NumImages { get; set; } = 1;

        [JsonPropertyName("style")]
        public string Style { get; set; } = "ANIME";

        [JsonPropertyName("width")]
        public int Width { get; set; } = 1280;

        [JsonPropertyName("height")]
        public int Height { get; set; } = 720;

        [JsonPropertyName("generateParams")]
        public GenerateParams GenerateParams { get; set; } = new GenerateParams();

    }
}
