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
    public class DeleteFriend
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
                var fiestaUser = await _db.FiestaUsers.Include(x => x.Friends).SingleOrNotFoundAsync(x => x.Id == request.UserId, cancellationToken);
                var fiestaFriend = await _db.FiestaUsers.Include(x => x.Friends).SingleOrNotFoundAsync(x => x.Id == request.FriendId, cancellationToken);

                var notiifcationModel = new FriendRemoved(fiestaUser);
                var notification = _db.Notifications.Add(new Notification(fiestaFriend.Id, notiifcationModel)).Entity;
                await _notificationsHub.Notify(notification);

                fiestaUser.RemoveFriend(fiestaFriend);
                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
