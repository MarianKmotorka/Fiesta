using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    [Authorize(nameof(FiestaRoleEnum.BasicUser))]
    [Route("api/events")]
    public class EventsController : BaseController
    {
        [HttpPost("create")]
        public async Task<ActionResult<CreateEvent.Response>> CreateEvent(CreateEvent.Command command, CancellationToken cancellationToken)
        {
            command.OrganizerId = CurrentUserService.UserId;
            var response = await Mediator.Send(command, cancellationToken);
            return Ok(response);
        }

        [HttpPost("{id}/invitations")]
        public async Task<ActionResult> Invite(string id, InviteUsersToEvent.Command command, CancellationToken cancellationToken)
        {
            command.CurrentUserId = CurrentUserService.UserId;
            command.EventId = id;
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/invitations/delete")]
        public async Task<ActionResult> DeleteInvitations(string id, DeleteEventInvitations.Command command, CancellationToken cancellationToken)
        {
            command.CurrentUserId = CurrentUserService.UserId;
            command.EventId = id;
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/invitations/reply")]
        public async Task<ActionResult> ReplyToInvitation(string id, ReplyToEventInvitation.Command command, CancellationToken cancellationToken)
        {
            command.CurrentUserId = CurrentUserService.UserId;
            command.EventId = id;
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/join-requests")]
        public async Task<ActionResult> RequestToJoin(string id, CancellationToken cancellationToken)
        {
            await Mediator.Send(
                new RequestToJoinEvent.Command { CurrentUserId = CurrentUserService.UserId, EventId = id },
                cancellationToken
                );

            return NoContent();
        }

        [HttpPost("{id}/join-requests/reply")]
        public async Task<ActionResult> ReplyToJoinRequest(string id, ReplyToEventJoinRequest.Command command, CancellationToken cancellationToken)
        {
            command.EventId = id;
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/join-requests/delete")]
        public async Task<ActionResult> DeleteJoinRequest(string id, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteEventJoinRequest.Command { CurrentUserId = CurrentUserService.UserId, EventId = id }, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/attendees/delete")]
        public async Task<ActionResult> ReplyToJoinRequest(string id, DeleteEventAttendees.Command command, CancellationToken cancellationToken)
        {
            command.EventId = id;
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/attendees/query")]
        public async Task<ActionResult<QueryResponse<UserDto>>> GetAttendees(string id, GetEventAttendees.Query request, CancellationToken cancellationToken)
        {
            request.EventId = id;
            var result = await Mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QueryResponse<UserDto>>> GetDetail(string id, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetEventDetail.Query { Id = id }, cancellationToken);
            return Ok(result);
        }
    }
}
