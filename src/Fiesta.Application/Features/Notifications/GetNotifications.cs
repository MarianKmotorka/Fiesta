using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Domain.Entities.Notifications;
using MediatR;

namespace Fiesta.Application.Features.Notifications
{
    public class GetNotifications
    {
        public class Query : IRequest<SkippedItemsResponse<NotificationDto>>
        {
            public SkippedItemsDocument SkippedItemsDocument { get; set; } = new();

            public string CurrentUserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, SkippedItemsResponse<NotificationDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<SkippedItemsResponse<NotificationDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var notifications = await _db.Notifications
                    .Where(x => x.UserId == request.CurrentUserId)
                    .OrderByDescending(x => x.CreatedAt)
                    .BuildResponse(request.SkippedItemsDocument, cancellationToken);

                var mappedEntries = notifications.Entries.Select(x => new NotificationDto
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    Seen = x.Seen,
                    Type = x.Type,
                    Model = x.GetModel<object>()
                });

                return new SkippedItemsResponse<NotificationDto>(mappedEntries)
                {
                    Skip = notifications.Skip,
                    Take = notifications.Take,
                    TotalEntries = notifications.TotalEntries
                };
            }
        }

        public class NotificationDto
        {
            public long Id { get; set; }

            public NotificationType Type { get; set; }

            public bool Seen { get; set; }

            public DateTime CreatedAt { get; set; }

            public object Model { get; set; }
        }
    }
}
