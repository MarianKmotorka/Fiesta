using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    [Route("api/users")]
    public class UsersController : BaseController
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDetail.Response>> GetUserDetail(string id, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(new GetUserDetail.Query { Id = id }, cancellationToken);
            return Ok(response);
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("{id}/profile-picture")]
        public async Task<ActionResult<UploadProfilePicture.Response>> UploadProfilePicture(string id, [FromForm] UploadProfilePicture.Command query, CancellationToken cancellationToken)
        {
            query.UserId = id;
            var response = await Mediator.Send(query, cancellationToken);
            return Ok(response);
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpDelete("{id}/profile-picture")]
        public async Task<ActionResult> DeleteProfilePicture(string id, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteProfilePicture.Command() { UserId = id }, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateMyProfile(string id, UpdateUser.Command query, CancellationToken cancellationToken)
        {
            query.UserId = id;
            var response = await Mediator.Send(query, cancellationToken);
            return Ok(response);
        }
    }
}
