using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Domain.Entities.Users;
using MediatR;

namespace Fiesta.Application.Users.EventHandlers
{
    public class OnAuthUserCreatedCreateFiestaUserEventHanlder : INotificationHandler<AuthUserCreatedEvent>
    {
        private readonly IFiestaDbContext _db;

        public OnAuthUserCreatedCreateFiestaUserEventHanlder(IFiestaDbContext db)
        {
            _db = db;
        }

        public async Task Handle(AuthUserCreatedEvent notification, CancellationToken cancellationToken)
        {
            var fiestaUser = FiestaUser.CreateWithId(notification.UserId, notification.Email);

            fiestaUser.FirstName = notification.FirstName;
            fiestaUser.LastName = notification.LastName;
            fiestaUser.PictureUrl = notification.PictureUrl;

            _db.FiestaUsers.Add(fiestaUser);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
