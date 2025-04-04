using ParsingScenario.Abstractions.SOVAModels;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TextToSpeech.Interfaces;

namespace TextToSpeech.TTSWorkers
{
    public class SovaWorker : ITTSWorker
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        public SovaWorker(string baseUrl)
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

            var requestModel = new GenerationRequestModel() { Text = text, Voice = voicebank };

            var content = new StringContent(JsonSerializer.Serialize(requestModel, options));

            using var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
            request.Content = content;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            var res = await response.Content.ReadAsStringAsync();

            var cont = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return (null, false);
            }

            var result = await response.Content.ReadFromJsonAsync<GenerationResponseModel>();


            return (result!.Audios[0].Audio, true);
        }
    }
}
