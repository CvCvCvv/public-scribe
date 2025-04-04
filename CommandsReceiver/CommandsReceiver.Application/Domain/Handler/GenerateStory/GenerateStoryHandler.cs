using CommandsReceiver.Application.Abstractions;
using CommandsReceiver.Application.Domain.Requests.GenerateStory;
using CommandsReceiver.Application.Domain.Responses.GenerateStory;
using MediatR;
using System.Text.Json;

namespace CommandsReceiver.Application.Domain.Handler.GenerateStory
{
    public class GenerateStoryHandler : IRequestHandler<GenerateStoryRequest, GenerateStoryResponse>
    {
        private readonly IRabbitMqService _rabbitMqService;
        public GenerateStoryHandler(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public async Task<GenerateStoryResponse> Handle(GenerateStoryRequest request, CancellationToken cancellationToken)
        {
            _rabbitMqService.SendMessage(JsonSerializer.Serialize(request));
            var count = _rabbitMqService.CountInQueue() + 1;

            return new GenerateStoryResponse() { Success = true, QueuePosition = count };
        }
    }
}
