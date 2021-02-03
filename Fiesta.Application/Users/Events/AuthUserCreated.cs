using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Domain.Entities;
using MediatR;

namespace Fiesta.Application.Users.Events
{
    public class AuthUserCreatedEvent : INotification
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PictureUrl { get; set; }
    }

    public class AuthUserCreatedEventHanlder : INotificationHandler<AuthUserCreatedEvent>
    {
        private readonly IFiestaDbContext _db;

        public AuthUserCreatedEventHanlder(IFiestaDbContext db)
        {
            _db = db;
        }

        public async Task Handle(AuthUserCreatedEvent notification, CancellationToken cancellationToken)
        {
            var fiestaUser = new FiestaUser(notification.Email)
            {
                FirstName = notification.FirstName,
                LastName = notification.LastName,
                PictureUrl = notification.PictureUrl
            };

            _db.FiestaUsers.Add(fiestaUser);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
