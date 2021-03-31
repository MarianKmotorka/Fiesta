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
    public class GetFriends
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
                return await _db.UserFriends.Where(x => x.UserId == request.Id)
                    .Select(x => new UserDto
                    {
                        Id = x.Friend.Id,
                        Username = x.Friend.Username,
                        PictureUrl = x.Friend.PictureUrl
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
