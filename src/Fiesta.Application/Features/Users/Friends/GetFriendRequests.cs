using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Domain.Entities.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users.Friends
{
    public class GetFriendRequests
    {
        public class Query : IRequest<Response>
        {
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
                var friendShips = _db.FriendRequests.Include(x => x.From).Where(x => x.ToId == request.Id);
                var friendRequests = friendShips.Select(x => x.From).ToList();

                return new Response
                {
                    FriendRequests = friendRequests
                };
            }
        }

        public class Response
        {
            public List<FiestaUser> FriendRequests { get; set; }
        }
    }
}
