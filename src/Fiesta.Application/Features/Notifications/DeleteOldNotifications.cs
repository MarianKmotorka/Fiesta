using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Notifications
{
    public class DeleteOldNotifications
    {
        public class Command : IRequest
        {
            public TimeSpan DeleteAfter { get; set; }

            public int BatchSize { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IFiestaDbContext _db;
            private readonly IDateTimeProvider _timeProvider;

            public Handler(IFiestaDbContext db, IDateTimeProvider timeProvider)
            {
                _db = db;
                _timeProvider = timeProvider;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                while (true)
                {
                    var deleteNotificationsBeforeDate = _timeProvider.UtcNow.Subtract(request.DeleteAfter);
                    var toDelete = await _db.Notifications
                        .Where(x => x.CreatedAt < deleteNotificationsBeforeDate)
                        .Take(request.BatchSize)
                        .ToListAsync(cancellationToken);

                    if (!toDelete.Any())
                        break;

                    _db.Notifications.RemoveRange(toDelete);
                    await _db.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}
