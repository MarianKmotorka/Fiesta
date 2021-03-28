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

        [HttpPost("{id}/invite")]
        public async Task<ActionResult> Invite(string id, InviteUsersToEvent.Command command, CancellationToken cancellationToken)
        {
            command.CurrentUserId = CurrentUserService.UserId;
            command.EventId = id;
            var response = await Mediator.Send(command, cancellationToken);
            return Ok(response);
        }
    }
}
