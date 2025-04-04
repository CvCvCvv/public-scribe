using CommandsReceiver.Application.Domain.Responses.GenerateStory;
using MediatR;

namespace CommandsReceiver.Application.Domain.Requests.GenerateStory
{
    public class GenerateStoryRequest : IRequest<GenerateStoryResponse>
    {
        public string Theme { get; set; } = null!;
        public string Author { get; set; } = "Anonymus";
    }
}
