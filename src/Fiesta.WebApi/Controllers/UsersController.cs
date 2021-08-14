using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Users;
using Fiesta.Application.Features.Users.Friends;
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
            var response = await Mediator.Send(new GetUserDetail.Query { CurrentUserId = CurrentUserService.UserId, Id = id }, cancellationToken);
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
        public async Task<ActionResult> UpdateProfile(string id, UpdateUser.Command query, CancellationToken cancellationToken)
        {
            query.UserId = id;
            var response = await Mediator.Send(query, cancellationToken);
            return Ok(response);
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpGet("selector")]
        public async Task<ActionResult<List<UserDto>>> UsersSelector([FromQuery] UsersSelector.Query query, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("{id}/friends/query")]
        public async Task<ActionResult<QueryResponse<GetFriends.ResponseDto>>> GetFriends(string id, string search, GetFriends.Query query, CancellationToken cancellationToken)
        {
            query.Id = id;
            query.CurrentUserId = CurrentUserService.UserId;
            query.Search = search;
            var response = await Mediator.Send(query, cancellationToken);
            return Ok(response);
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("{id}/friend-requests/query")]
        public async Task<ActionResult<SkippedItemsResponse<FriendRequestDto>>> GetFriendRequests(string id, GetFriendRequests.Query query, CancellationToken cancellationToken)
        {
            query.Id = id;
            var response = await Mediator.Send(query, cancellationToken);
            return Ok(response);
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("{id}/events/query")]
        public async Task<ActionResult<QueryResponse<EventDto>>> GetUserEvents(string id, GetUserEvents.Query query, CancellationToken cancellationToken)
        {
            query.UserId = id;
            var response = await Mediator.Send(query, cancellationToken);
            return Ok(response);
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("{id}/organized-events")]
        public async Task<ActionResult<QueryResponse<EventDto>>> GetOrganizedEvents(string id, GetUserOrganizedEvents.Query query, CancellationToken cancellationToken)
        {
            query.UserId = id;
            query.CurrentUserId = CurrentUserService.UserId;
            query.Role = CurrentUserService.Role;
            var response = await Mediator.Send(query, cancellationToken);
            return Ok(response);
        }
    }
}
