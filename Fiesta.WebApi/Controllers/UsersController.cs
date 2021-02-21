using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Auth;
using Fiesta.Application.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    [Route("api/users")]
    public class UsersController : BaseController
    {
        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpGet("me")]
        public async Task<ActionResult<GetUserDetail.Query>> GetMyDetail(CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(new GetUserDetail.Query { Id = CurrentUserService.UserId }, cancellationToken);
            return Ok(response);
        }

        [Authorize(nameof(FiestaRoleEnum.Admin))]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDetail.Query>> GetUserDetail(string id, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(new GetUserDetail.Query { Id = id }, cancellationToken);
            return Ok(response);
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpDelete("delete-me")]
        public async Task<ActionResult> DeleteAccount(string password, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteAccountWithPassword.Query { UserId = CurrentUserService.UserId, Password = password }, cancellationToken);
            return NoContent();
        }
    }
}
