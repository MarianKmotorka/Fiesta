using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Users.Friends;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    [Route("api/friends")]
    public class FriendsController : BaseController
    {
        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("send-request")]
        public async Task<ActionResult> SendFriendRequest(SendFriendRequest.Command query, CancellationToken cancellationToken)
        {
            query.UserId = CurrentUserService.UserId;
            await Mediator.Send(query, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("confirm-request")]
        public async Task<ActionResult> ConfirmFriendRequest(ConfirmFriendRequest.Command query, CancellationToken cancellationToken)
        {
            query.UserId = CurrentUserService.UserId;
            await Mediator.Send(query, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFriend(string id, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteFriend.Command
            {
                UserId = CurrentUserService.UserId,
                FriendId = id
            }, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("unsend-request")]
        public async Task<ActionResult> UnsendFriendRequest(UnsendFriendRequest.Command query, CancellationToken cancellationToken)
        {
            query.UserId = CurrentUserService.UserId;
            await Mediator.Send(query, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("reject-request")]
        public async Task<ActionResult> RejectFriendRequest(RejectFriendRequest.Command query, CancellationToken cancellationToken)
        {
            query.UserId = CurrentUserService.UserId;
            await Mediator.Send(query, cancellationToken);
            return NoContent();
        }
    }
}
