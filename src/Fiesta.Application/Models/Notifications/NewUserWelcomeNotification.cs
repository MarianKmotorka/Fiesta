using Fiesta.Domain.Entities.Notifications;
using Fiesta.Domain.Entities.Users;

namespace Fiesta.Application.Models.Notifications
{
    public class NewUserWelcomeNotification : INotificationModel
    {
        public NewUserWelcomeNotification(FiestaUser user)
        {
            FirstName = user.FirstName;
        }

        public NewUserWelcomeNotification()
        {
        }

        public string FirstName { get; }

        public NotificationType Type => NotificationType.NewUserWelcome;
    }
}
