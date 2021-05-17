using System.Threading.Tasks;
using Fiesta.Application.Features.Notifications;
using Fiesta.Domain.Entities.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Fiesta.Application.Features.Common
{
    public static class NotificationExtensions
    {
        public static async Task Notify(this IHubContext<NotificationsHub, INotificationsClient> hub, Notification notification)
        {
            if (NotificationsHub.UserConnections.TryGetValue(notification.UserId, out var connectionIds))
                await hub.Clients.Clients(connectionIds).ReceiveNotification(NotificationDto.Map(notification));
        }
    }
}
