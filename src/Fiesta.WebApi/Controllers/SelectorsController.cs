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
    [Authorize(nameof(FiestaRoleEnum.BasicUser))]
    public class SelectorsController : BaseController
    {
        [HttpGet("events-and-users")]
        public async Task<ActionResult<List<EventsAndUsersSelector.ResponseDto>>> UsersSelector(string search, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new EventsAndUsersSelector.Query
            {
                Search = search,
                Role = CurrentUserService.Role,
                CurrentUserId = CurrentUserService.UserId
            }, cancellationToken);
            return Ok(result);
        }
    }
}
