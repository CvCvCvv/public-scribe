using Application.Abstractions.Consts;

namespace ImageGenerators.Helpers
{
    public class RemoveBackgroundService
    {
        private const int _numberAttempts = 5;
        private const string _endpoint = "http://localhost:7000/api/remove";
        private const string _useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:90.0) Gecko/20100101 Firefox/90.0";
        public RemoveBackgroundService() { }

        public async Task<(string, bool)> RemoveBackground(string backgroundPhoto)
        {
            if (!Settings.RemovingBg)
            {
                return (backgroundPhoto, true);
            }

            var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_useragent);

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _endpoint); ;

            MultipartFormDataContent form = new MultipartFormDataContent
            {
                { new ByteArrayContent(Convert.FromBase64String(backgroundPhoto)), "file", "file.jpg" }
            };

            request.Content = form;
            var response = await _httpClient.PostAsync(_endpoint, form);

            for (int i = 0; i < _numberAttempts && !response.IsSuccessStatusCode; i++)
            {
                using HttpRequestMessage rewriteRequest = new HttpRequestMessage(HttpMethod.Post, _endpoint); ;

                request.Content = form;
                response = await _httpClient.PostAsync(_endpoint, form);
            }

            var photo = Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync());

            return (photo, response.IsSuccessStatusCode);
        }
    }
}
