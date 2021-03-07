using Fiesta.Application.Features.Events;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.WebApi.Controllers
{
    [Route("api/events")]
    public class EventsController : BaseController
    {
        [HttpPost("create")]
        public async Task<ActionResult<CreateEvent.Command>> CreateEvent(CreateEvent.Command command, CancellationToken cancellationToken)
        {
            command.OrganizerId = CurrentUserService.UserId;
            var response = await Mediator.Send(command, cancellationToken);
            return Ok(response);
        }
    }
}
