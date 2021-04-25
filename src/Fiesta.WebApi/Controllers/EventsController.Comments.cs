using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Features.Events.Comments;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    public partial class EventsController
    {
        [HttpPost("{id}/comments")]
        public async Task<ActionResult> CreateComment(string id, CreateComment.Command request, CancellationToken cancellationToken)
        {
            request.CurrentUserId = CurrentUserService.UserId;
            request.EventId = id;
            var response = await Mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
