using ParsingScenario.Abstractions.SOVAModels;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TextToSpeech.Interfaces;

namespace TextToSpeech.TTSWorkers
{
    public class PiperWorker : ITTSWorker
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        public PiperWorker(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
        }
        public async Task<(string?, bool)> ToSpeech(string text, string voicebank)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };

            var requestModel = new GenerationRequestModel() { Text = text, Voice = GetVoiceBank(voicebank) };

            var content = new StringContent(JsonSerializer.Serialize(requestModel, options));

            using var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/api/tts");
            request.Content = content;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            var res = await response.Content.ReadAsStringAsync();

            var cont = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return (null, false);
            }

            var result = Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync());

            return (result, true);
        }

        private string GetVoiceBank(string gender)
        {
            return gender == "man" ? "ru_RU-dmitri-medium.onnx" : "ru_RU-irina-medium.onnx";
        }
    }
}