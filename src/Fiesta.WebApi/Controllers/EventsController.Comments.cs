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

        [HttpPost("{id}/comments/query")]
        public async Task<ActionResult> GetComments(string id, GetComments.Query request, CancellationToken cancellationToken)
        {
            request.EventId = id;
            var response = await Mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("{eventId}/comments/{commentId}")]
        public async Task<ActionResult> DeleteComment([FromRoute] DeleteComment.Command request, CancellationToken cancellationToken)
        {
            await Mediator.Send(request, cancellationToken);
            return NoContent();
        }
    }
}
