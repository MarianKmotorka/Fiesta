using System;
using Fiesta.Domain.Entities.Notifications;

namespace Fiesta.Application.Features.Notifications
{
    public class NotificationDto
    {
        public long Id { get; set; }

        public NotificationType Type { get; set; }

        public bool Seen { get; set; }

        public DateTime CreatedAt { get; set; }

        public object Model { get; set; }

        public static NotificationDto Map(Notification notification)
            => new NotificationDto
            {
                Id = notification.Id,
                Seen = notification.Seen,
                Type = notification.Type,
                CreatedAt = notification.CreatedAt,
                Model = notification.GetModel<object>(),
            };
    }
}
