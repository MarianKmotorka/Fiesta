using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users.Friends
{
    public class GetFriends
    {
        public class Query : IRequest<List<ResponseDto>>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            [JsonIgnore]
            public string CurrentUserId { get; set; }
            [JsonIgnore]
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<ResponseDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<List<ResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var usersQuery = _db.UserFriends.AsNoTracking().Where(x => x.UserId == request.UserId)
                   .Select(x => new ResponseDto
                   {
                       Id = x.Friend.Id,
                       Username = x.Friend.Username,
                       FirstName = x.Friend.FirstName,
                       LastName = x.Friend.LastName,
                       PictureUrl = x.Friend.PictureUrl
                   });

                if (!string.IsNullOrEmpty(request.Search))
                    usersQuery = usersQuery.Where(x => (x.FirstName + " " + x.LastName).Contains(request.Search));

                var users = await usersQuery.OrderBy(x => x.Username).Take(25).ToListAsync(cancellationToken);

                foreach (var user in users)
                {
                    if (await _db.UserFriends.AnyAsync(x => x.UserId == request.CurrentUserId && x.FriendId == user.Id, cancellationToken))
                        user.FriendStatus = FriendStatus.Friend;
                    else if (await _db.FriendRequests.AnyAsync(x => x.FromId == request.CurrentUserId && x.ToId == user.Id, cancellationToken))
                        user.FriendStatus = FriendStatus.FriendRequestSent;
                    else if (await _db.FriendRequests.AnyAsync(x => x.FromId == user.Id && x.ToId == request.CurrentUserId, cancellationToken))
                        user.FriendStatus = FriendStatus.FriendRequestRecieved;
                }

                return users;
            }
        }

        public class ResponseDto
        {
            public string Id { get; set; }

            [JsonIgnore]
            public string FirstName { get; set; }

            [JsonIgnore]
            public string LastName { get; set; }

            public string FullName { get => FirstName is null ? null : $"{FirstName} {LastName}"; }

            public string Username { get; set; }

            public string PictureUrl { get; set; }

            public FriendStatus FriendStatus { get; set; }
        }
    }
}
