using Application.Abstractions.Consts;
using Application.Abstractions.MistralModels;
using LLMWorkers.Interfaces;
using Mistral.SDK;
using Mistral.SDK.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LLMWorkers.Workers
{
    public class MistralWorker : ILLMWorker
    {
        private const string _url = "https://api.mistral.ai/v1/agents/completions";

        /// <summary>
        ///Линейный агент
        /// </summary>
        private const string _linealAgent = "ag:00";
        /// <summary>
        /// Агент с ветвлением диалогов
        /// </summary>
        private const string _treeAgent = "ag:01";

        private readonly MistralClient _mistralClient;
        private readonly HttpClient _httpClient;
        private readonly string _key;
        public MistralWorker(string apiKey)
        {
            _key = apiKey;
            _mistralClient = new MistralClient(apiKeys: new APIAuthentication(apiKey));
            _httpClient = new HttpClient();
        }

        public async Task<string> GetScenario(string theme)
        {
            // В библиотеке не реализованы agents, поэтому просто запрос 
            using var request = new HttpRequestMessage(HttpMethod.Post, _url);
            var agent = Settings.BranchingDialogues ? _treeAgent : _linealAgent;

            var message = new AgentRequest() { Agent = agent, Messages = new() { new() { Role = "user", Content = theme } } };

            var stringContent = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
            request.Content = stringContent;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _key);

            using var responseMessage = await _httpClient.SendAsync(request);
            var response = await responseMessage.Content.ReadFromJsonAsync<ChatCompletionResponse>()!;

            return response!.Choices.First().Message.Content;
        }
    }
}
