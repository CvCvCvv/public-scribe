using System.Text.Json.Serialization;

namespace ParsingScenario.Abstractions.SOVAModels
{
    public class GenerationResponseModel
    {
        [JsonPropertyName("response")]
        public List<ResponseAudio> Audios { get; set; } = new();
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }
    }

    public class ResponseAudio
    {
        [JsonPropertyName("duration_s")]
        public float DurationS { get; set; }
        [JsonPropertyName("response_audio")]
        public string Audio { get; set; } = null!;
        [JsonPropertyName("response_audio_url")]
        public string ResponseAudioUrl { get; set; } = null!;
        [JsonPropertyName("sample_rate")]
        public int SampleRate { get; set; }
        [JsonPropertyName("synthesis_time")]
        public double SynthesisTime { get; set; }
        [JsonPropertyName("voice")]
        public string Voice { get; set; } = null!;
    }
}
