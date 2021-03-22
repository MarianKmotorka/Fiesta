using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Selectors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    [Route("api/selectors")]
    public class SelectorsController : BaseController
    {
        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpGet("events-and-users")]
        public async Task<ActionResult<List<EventsAndUsersSelector.ResponseDto>>> UsersSelector([FromQuery] EventsAndUsersSelector.Query query, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
