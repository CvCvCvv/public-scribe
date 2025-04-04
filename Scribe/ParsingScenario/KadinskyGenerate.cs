using Application.Abstractions.KadinskyModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text.Json;

namespace ParsingScenario
{
    internal class KadinskyGenerate
    {
        private const string _api = "https://api-key.fusionbrain.ai/";
        private const string _getModelsEndpoint = "key/api/v1/models";
        private const string _textToImageEndpoint = "key/api/v1/text2image/run";
        private const string _checkEndpoint = "key/api/v1/text2image/status/";

        private readonly string _key;
        private readonly string _secret;
        private readonly HttpClient _httpClient;

        public KadinskyGenerate(string key, string secret)
        {
            _key = key;
            _secret = secret;
            _httpClient = new HttpClient();
        }

        public async Task<int?> GetModel()
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _api + _getModelsEndpoint);
            request.Headers.Add("X-Key", "Key " + _key);
            request.Headers.Add("X-Secret", "Secret " + _secret);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<List<GetModels>>();

            return result![0].Id;
        }

        public async Task<string?> Generate(string promt, int model, int width = 1024, int height = 1024, int count = 1, string style = "ANIME")
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _api + _textToImageEndpoint);
            request.Headers.Add("X-Key", "Key " + _key);
            request.Headers.Add("X-Secret", "Secret " + _secret);

            var paramic = new ParamsImage() { Height = height, Width = width, Style = style };
            paramic.GenerateParams.Query = promt;
            
            MultipartFormDataContent form = new MultipartFormDataContent
            {
                { new StringContent(JsonSerializer.Serialize(paramic), new MediaTypeHeaderValue("application/json")), "params" },
                { new StringContent(model.ToString()), "model_id" }
            };

            form.Headers.Add("X-Key", "Key " + _key);
            form.Headers.Add("X-Secret", "Secret " + _secret);

            var response = await _httpClient.PostAsync(_api + _textToImageEndpoint, form);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<Generation>();

            return result!.Uuid;
        }

        public async Task<string?> CheckGenerate(string uuid)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _api + _checkEndpoint + uuid);
            request.Headers.Add("X-Key", "Key " + _key);
            request.Headers.Add("X-Secret", "Secret " + _secret);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<Generation>();
            if (result!.Status == "DONE")
                return result.Images[0];

            return result.Uuid;
        }
    }
}
