using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Models.Notifications;
using Fiesta.Domain.Entities.Notifications;
using Fiesta.Domain.Entities.Users;
using MediatR;

namespace Fiesta.Application.Features.Users.EventHandlers
{
    public class OnAuthUserCreatedCreateFiestaUserEventHandler : INotificationHandler<AuthUserCreatedEvent>
    {
        private readonly IFiestaDbContext _db;

        public OnAuthUserCreatedCreateFiestaUserEventHandler(IFiestaDbContext db)
        {
            _db = db;
        }

        public async Task Handle(AuthUserCreatedEvent @event, CancellationToken cancellationToken)
        {
            var fiestaUser = FiestaUser.CreateWithId(@event.UserId, @event.Email, @event.Username);

            fiestaUser.FirstName = @event.FirstName;
            fiestaUser.LastName = @event.LastName;
            fiestaUser.PictureUrl = @event.PictureUrl;
            _db.FiestaUsers.Add(fiestaUser);

            // NOTE: Calling save before saving notification so FiestaUser.Id would be filled
            await _db.SaveChangesAsync(cancellationToken);

            var notification = new Notification(fiestaUser, new NewUserWelcomeNotification(fiestaUser));
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
