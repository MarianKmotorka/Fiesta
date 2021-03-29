using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
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

        [HttpPost("{id}/delete-invitations")]
        public async Task<ActionResult> DeleteInvitations(string id, DeleteEventInvitations.Command command, CancellationToken cancellationToken)
        {
            command.CurrentUserId = CurrentUserService.UserId;
            command.EventId = id;
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/invitation-reply")]
        public async Task<ActionResult> InvitationReply(string id, ReplyToEventInvitation.Command command, CancellationToken cancellationToken)
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

        [HttpPost("{id}/join-request-reply")]
        public async Task<ActionResult> ReplyToJoinRequest(string id, ReplyToEventJoinRequest.Command command, CancellationToken cancellationToken)
        {
            command.EventId = id;
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/delete-attendees")]
        public async Task<ActionResult> ReplyToJoinRequest(string id, DeleteEventAttendees.Command command, CancellationToken cancellationToken)
        {
            command.EventId = id;
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
