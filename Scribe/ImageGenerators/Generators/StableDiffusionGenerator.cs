using ImageGenerators.Interfaces;
using System.Net.Http.Json;

namespace ImageGenerators.Generators
{
    public class StableDiffusionGenerator : IGeneratorImage
    {
        private const int _numberAttempts = 5;
        private readonly string _baseUrl;
        public StableDiffusionGenerator(string url)
        {
            _baseUrl = url;
        }

        public async Task<(string?, bool)> GenerateImage2ImageToBase64(string prompt, string image_b64, int width, int height, double strength = 0.85)
        {
            var isSuccess = false;
            var httpClient = new HttpClient();

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
            request.Content = JsonContent.Create(new { prompt, width, height, image_b64, strength });

            var response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsByteArrayAsync();
            for (int i = 0; i < _numberAttempts && (!response.IsSuccessStatusCode || !CheckImageValid(content)); i++)
            {
                using HttpRequestMessage rewriteRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
                rewriteRequest.Content = JsonContent.Create(new { prompt, width, height, image_b64, strength });

                response = await httpClient.SendAsync(rewriteRequest);
                content = await response.Content.ReadAsByteArrayAsync();
            }


            var result = Convert.ToBase64String(content);

            if (response.IsSuccessStatusCode)
                isSuccess = true;

            return (result, isSuccess);
        }

        public async Task<(string?, bool)> GenerateImageToBase64(string prompt, int width, int height, string style = "UHD")
        {
            var isSuccess = false;
            var httpClient = new HttpClient();

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
            request.Content = JsonContent.Create(new { prompt, width, height });

            var response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsByteArrayAsync();
            for (int i = 0; i < _numberAttempts && (!response.IsSuccessStatusCode || !CheckImageValid(content)); i++)
            {
                using HttpRequestMessage rewriteRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
                rewriteRequest.Content = JsonContent.Create(new { prompt, width, height });

                response = await httpClient.SendAsync(rewriteRequest);
                content = await response.Content.ReadAsByteArrayAsync();
            }


            var result = Convert.ToBase64String(content);

            if (response.IsSuccessStatusCode)
                isSuccess = true;


            return (result, isSuccess);
        }

        private bool CheckImageValid(byte[] image)
        {
            var c = image.Where(a => a == 0).ToList().Count;
            var l = image.Length;
            return image.Where(a => a == 0).ToList().Count < image.Length * 0.6;
        }
    }
}
