using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

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
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateUser.Query>> UpdateUser(UpdateUser.Query query, CancellationToken cancellationToken)
        {
            query.UserId = CurrentUserService.UserId;
            var response = await Mediator.Send(query);
            return Ok(response);
        }
    }
}
