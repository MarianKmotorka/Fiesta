using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Notifications;
using Fiesta.Application.Models.Notifications;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users.Friends
{
    public class ConfirmFriendRequest
    {
        public class Command : IRequest<Unit>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            public string FriendId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IFiestaDbContext _db;
            private readonly IHubContext<NotificationsHub, INotificationsClient> _notificationsHub;

            public Handler(IFiestaDbContext db, IHubContext<NotificationsHub, INotificationsClient> notificationsHub)
            {
                _db = db;
                _notificationsHub = notificationsHub;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var friendRequest = await _db.FriendRequests
                    .Include(x => x.From)
                    .Include(x => x.To)
                    .SingleOrNotFoundAsync(x => x.FromId == request.FriendId && x.ToId == request.UserId, cancellationToken);

                _db.FriendRequests.Remove(friendRequest);

                var notiifcationModel = new FriendRequestReply(friendRequest.To, accepted: true);
                var notification = _db.Notifications.Add(new Notification(friendRequest.FromId, notiifcationModel)).Entity;
                await _notificationsHub.Notify(notification);

                friendRequest.To.AddFriend(friendRequest.From);
                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
