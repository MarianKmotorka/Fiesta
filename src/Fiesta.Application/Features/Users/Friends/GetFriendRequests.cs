using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Utils;
using MediatR;

namespace Fiesta.Application.Features.Users.Friends
{
    public class GetFriendRequests
    {
        public class Query : IRequest<QueryResponse<UserDto>>
        {
            [JsonIgnore]
            public string Id { get; set; }
            public QueryDocument QueryDocument { get; set; } = new();
        }

        public class Handler : IRequestHandler<Query, QueryResponse<UserDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResponse<UserDto>> Handle(Query request, CancellationToken cancellationToken)
            {

                return await _db.FriendRequests.Where(x => x.ToId == request.Id)
                    .Select(x => new UserDto
                    {
                        Id = x.From.Id,
                        Username = x.From.Username,
                        PictureUrl = x.From.PictureUrl
                    })
                    .BuildResponse(request.QueryDocument, cancellationToken);
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => Task.FromResult(currentUserService.IsResourceOwnerOrAdmin(request.Id));
        }
    }
}
