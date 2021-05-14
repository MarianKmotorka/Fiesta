using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Domain.Entities.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Notifications
{
    public class GetNotifications
    {
        public class Query : IRequest<SkippedItemsResponse<NotificationDto, AdditionalData>>
        {
            public SkippedItemsDocument SkippedItemsDocument { get; set; } = new();

            public string CurrentUserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, SkippedItemsResponse<NotificationDto, AdditionalData>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<SkippedItemsResponse<NotificationDto, AdditionalData>> Handle(Query request, CancellationToken cancellationToken)
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

                var unseenCount = await _db.Notifications.Where(x => x.UserId == request.CurrentUserId).CountAsync(x => !x.Seen, cancellationToken);

                return new SkippedItemsResponse<NotificationDto, AdditionalData>(mappedEntries, new AdditionalData { UnseenCount = unseenCount })
                {
                    Skip = notifications.Skip,
                    Take = notifications.Take,
                    TotalEntries = notifications.TotalEntries
                };
            }
        }

        public class AdditionalData
        {
            public int UnseenCount { get; set; }
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
