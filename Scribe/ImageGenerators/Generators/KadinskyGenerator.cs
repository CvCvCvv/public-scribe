using Application.Abstractions.Domains;
using Application.Abstractions.KadinskyModel;
using ImageGenerators.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ImageGenerators.Generators
{
    public class KadinskyGenerator : IGeneratorImage
    {
        private const string _api = "https://api-key.fusionbrain.ai/";
        private const string _getModelsEndpoint = "key/api/v1/models";
        private const string _textToImageEndpoint = "key/api/v1/text2image/run";
        private const string _checkEndpoint = "key/api/v1/text2image/status/";

        private readonly string _key;
        private readonly string _secret;
        private readonly HttpClient _httpClient;

        public KadinskyGenerator(string key, string secret)
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

        public async Task<string?> Generate(string promt, int model, int width = 1024, int height = 1024, int count = 1, string style = "REALISTIC")
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


        /// <summary>
        /// У кадинского не сущесвует такой модели
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="image_b64"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<(string?, bool)> GenerateImage2ImageToBase64(string prompt, string image_b64, int width, int height, double strength = 0.75)
        {
            throw new NotImplementedException();
        }

        public async Task<(string?, bool)> GenerateImageToBase64(string prompt, int width, int height, string style = "UHD")
        {
            var isSuccess = false;

            var model = await GetModel();

            string? photo = "";
            var uuid = await Generate(prompt + KadinskyPromtHelper.CharacterGenerate, (int)model, width: 334, height: 700, style: style);
            if (uuid != null)
            {

                for (int i = 0; i < 10 || uuid == photo; i++)
                {
                    photo = await CheckGenerate(uuid);

                    if (photo is null)
                        break;

                    Thread.Sleep(1000);
                }
            }
            if (photo != uuid)
            {
                await Console.Out.WriteLineAsync($"Generate image kadinsky");
                isSuccess = true;
            }
            return (photo, isSuccess);
        }
    }
}
