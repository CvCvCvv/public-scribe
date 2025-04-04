using LLMWorkers.Helpers;
using LLMWorkers.Interfaces;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace LLMWorkers.Workers
{
    public class ChatGPTWorker : ILLMWorker
    {
        private readonly string _key;
        private const string _endpoint = "https://fresedgpt.space/v1";
        private const string _model = "chatgpt-4o-latest";
        private readonly ChatClient _textClient;

        public ChatGPTWorker(string key)
        {
            _key = key;

            var clientOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri(_endpoint),
            };

            _textClient = new ChatClient(_model, new ApiKeyCredential(_key), clientOptions);
        }

        public async Task<string> GetScenario(string theme)
        {
            var complectation = (await _textClient.CompleteChatAsync(TemplateContent.Act + theme)).Value;

            return complectation.Content[0].Text;
        }
    }
}
