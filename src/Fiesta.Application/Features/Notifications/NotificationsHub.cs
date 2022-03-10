using System.Linq;
using System.Threading.Tasks;
using Fiesta.Application.Common;
using Fiesta.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Notifications
{
    [Authorize]
    public class NotificationsHub : HubBase<INotificationsClient>
    {
        private readonly IFiestaDbContext _db;

        public NotificationsHub(ICurrentUserService currentUser, IFiestaDbContext db) : base(currentUser)
        {
            _db = db;
        }

        public async Task SetSeen(long id)
        {
            var notification = await _db.Notifications.SingleOrDefaultAsync(x => x.Id == id && x.UserId == _currentUser.UserId);
            if (notification is null)
                return;

            notification.SetSeen();
            await _db.SaveChangesAsync();
        }

        public async Task SetAllSeen()
        {
            var unseen = await _db.Notifications.Where(x => x.UserId == _currentUser.UserId && !x.Seen).ToListAsync();
            unseen.ForEach(x => x.SetSeen());
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            var toDelete = await _db.Notifications.Where(x => x.UserId == _currentUser.UserId).ToListAsync();
            _db.Notifications.RemoveRange(toDelete);
            await _db.SaveChangesAsync();
        }
    }

    public interface INotificationsClient
    {
        Task ReceiveNotification(NotificationDto notification);
    }
}
