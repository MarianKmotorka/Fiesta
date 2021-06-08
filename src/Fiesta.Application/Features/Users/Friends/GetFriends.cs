using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users.Friends
{
    public class GetFriends
    {
        public class Query : IRequest<QueryResponse<ResponseDto>>
        {
            [JsonIgnore]
            public string Id { get; set; }
            [JsonIgnore]
            public string Search { get; set; }
            [JsonIgnore]
            public string CurrentUserId { get; set; }
            public QueryDocument QueryDocument { get; set; } = new();
        }

        public class Handler : IRequestHandler<Query, QueryResponse<ResponseDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResponse<ResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var usersQuery = _db.UserFriends.AsNoTracking().Where(x => x.UserId == request.Id)
                   .Select(x => new ResponseDto
                   {
                       Id = x.Friend.Id,
                       Username = x.Friend.Username,
                       FirstName = x.Friend.FirstName,
                       LastName = x.Friend.LastName,
                       PictureUrl = x.Friend.PictureUrl,
                       FriendStatus = x.Friend.Friends.Any(x => x.FriendId == request.CurrentUserId)
                       ? FriendStatus.Friend
                       : x.Friend.RecievedFriendRequests.Any(x => x.FromId == request.CurrentUserId)
                       ? FriendStatus.FriendRequestSent
                       : x.Friend.SentFriendRequests.Any(x => x.ToId == request.CurrentUserId)
                       ? FriendStatus.FriendRequestRecieved
                       : FriendStatus.None
                   });

                if (!string.IsNullOrEmpty(request.Search))
                    usersQuery = usersQuery.Where(x => (x.FirstName + " " + x.LastName).Contains(request.Search) || (x.Username).Contains(request.Search));

                return await usersQuery.BuildResponse(request.QueryDocument, cancellationToken);
            }
        }

        public class ResponseDto : UserDto
        {
            public FriendStatus FriendStatus { get; set; }
        }
    }
}
