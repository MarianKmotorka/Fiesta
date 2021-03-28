using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users
{
    public class GetUserDetail
    {
        public class Query : IRequest<Response>
        {
            public string CurrentUserId { get; set; }
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _db.FiestaUsers.SingleOrNotFoundAsync(x => x.Id == request.Id, cancellationToken);
                var numberOfFriends = await _db.UserFriends.CountAsync(x => x.UserId == request.Id, cancellationToken);

                var friendStatus = FriendStatus.None;

                if (await _db.UserFriends.AnyAsync(x => x.UserId == request.CurrentUserId && x.FriendId == request.Id, cancellationToken))
                    friendStatus = FriendStatus.Friend;
                else if (await _db.FriendRequests.AnyAsync(x => x.FromId == request.CurrentUserId && x.ToId == request.Id, cancellationToken))
                    friendStatus = FriendStatus.FriendRequestSent;
                else if (await _db.FriendRequests.AnyAsync(x => x.FromId == request.Id && x.ToId == request.CurrentUserId, cancellationToken))
                    friendStatus = FriendStatus.FriendRequestRecieved;

                return new Response
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Username = user.Username,
                    PictureUrl = user.PictureUrl,
                    Bio = user.Bio,
                    NumberOfFriends = numberOfFriends,
                    FriendStatus = friendStatus
                };
            }
        }

        public class Response
        {
            public string Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string FullName { get; set; }

            public string Username { get; set; }

            public string PictureUrl { get; set; }

            public string Bio { get; set; }

            public int NumberOfFriends { get; set; }

            public FriendStatus FriendStatus { get; set; }
        }

        public enum FriendStatus
        {
            None,
            Friend,
            FriendRequestSent,
            FriendRequestRecieved
        }
    }
}
