using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Utils;
using MediatR;

namespace Fiesta.Application.Features.Users.Friends
{
    public class GetFriendRequests
    {
        public class Query : IRequest<SkippedItemsResponse<FriendRequestDto>>
        {
            [JsonIgnore]
            public string Id { get; set; }
            public SkippedItemsDocument SkippedItemsDocument { get; set; } = new();
        }

        public class Handler : IRequestHandler<Query, SkippedItemsResponse<FriendRequestDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<SkippedItemsResponse<FriendRequestDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _db.FriendRequests.Where(x => x.ToId == request.Id)
                    .OrderByDescending(x => x.RequestedOn)
                    .Select(x => new FriendRequestDto
                    {
                        User = new()
                        {
                            Id = x.From.Id,
                            Username = x.From.Username,
                            FirstName = x.From.FirstName,
                            LastName = x.From.LastName,
                            PictureUrl = x.From.PictureUrl
                        },
                        RequestedOn = x.RequestedOn
                    })
                    .BuildResponse(request.SkippedItemsDocument, cancellationToken);
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => Task.FromResult(currentUserService.IsResourceOwnerOrAdmin(request.Id));
        }
    }
}
