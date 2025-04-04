using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using CommandsReceiver.Application.Domain.Requests.GenerateStory;
using CommandsReceiver.Application.Domain.Responses.GenerateStory;

namespace Commands.Receiver.Infrastructure.Api.Controllers
{
    public class GetStoryThemeController : BaseController
    {
        private readonly IMediator _mediator;

        public GetStoryThemeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("toQueueTheme")]
        [SwaggerResponse(StatusCodes.Status200OK, "Post 200 ToQueueTheme", typeof(GenerateStoryResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Post 400 ToQueueTheme", typeof(GenerateStoryResponse))]
        public async Task<IActionResult> ToQueueTheme(GenerateStoryRequest request)
        {
            var response = await _mediator.Send(request);

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
