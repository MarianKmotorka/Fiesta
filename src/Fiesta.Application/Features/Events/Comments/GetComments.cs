using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events.Comments.Common;
using Fiesta.Application.Features.Events.Common;
using MediatR;

namespace Fiesta.Application.Features.Events.Comments
{
    public class GetComments
    {
        public class Query : IRequest<SkippedItemsResponse<CommentDto>>
        {
            public SkippedItemsDocument SkippedItemsDocument { get; set; } = new();

            [JsonIgnore]
            public string EventId { get; set; }

            public string ParentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, SkippedItemsResponse<CommentDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<SkippedItemsResponse<CommentDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _db.EventComments
                    .Where(x => x.EventId == request.EventId)
                    .Where(x => x.ParentId == request.ParentId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new CommentDto
                    {
                        Id = x.Id,
                        Text = x.Text,
                        CreatedAt = x.CreatedAt,
                        IsEdited = x.IsEdited,
                        ParentId = x.ParentId,
                        ReplyCount = x.Replies.Count(),
                        Sender = new UserDto
                        {
                            Id = x.Sender.Id,
                            Username = x.Sender.Username,
                            PictureUrl = x.Sender.PictureUrl,
                            FirstName = x.Sender.FirstName,
                            LastName = x.Sender.LastName,
                        },
                    }).BuildResponse(request.SkippedItemsDocument, cancellationToken);
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public async Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => await Helpers.IsOrganizerOrAttendeeOrAdmin(request.EventId, db, currentUserService, cancellationToken);
        }
    }
}
