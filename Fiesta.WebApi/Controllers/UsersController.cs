using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    [Authorize]
    [Route("api/users")]
    public class UsersController : BaseController
    {
        [HttpGet("me")]
        public async Task<ActionResult<GetUserDetail.Query>> GetMyDetail(CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(new GetUserDetail.Query { Id = CurrentUserService.UserId }, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDetail.Query>> GetMyDetail(string id, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(new GetUserDetail.Query { Id = id }, cancellationToken);
            return Ok(response);
        }
    }
}
