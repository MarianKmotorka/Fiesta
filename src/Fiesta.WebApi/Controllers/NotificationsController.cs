using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    [Route("api/notifications")]
    [Authorize(nameof(FiestaRoleEnum.BasicUser))]
    public class NotificationsController : BaseController
    {
        [HttpPost("query")]
        public async Task<ActionResult<SkippedItemsResponse<GetNotifications.NotificationDto>>> Get(GetNotifications.Query request, CancellationToken cancellationToken)
        {
            request.CurrentUserId = CurrentUserService.UserId;
            var response = await Mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
