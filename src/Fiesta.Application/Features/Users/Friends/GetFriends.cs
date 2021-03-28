using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users.Friends
{
    public class GetFriends
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
                var user = await _db.FiestaUsers.Include(x => x.Friends).ThenInclude(x => x.Friend).SingleOrNotFoundAsync(x => x.Id == request.Id, cancellationToken);
                var friends = user.Friends.Select(x => x.Friend).ToList();

                return new Response
                {
                    Friends = friends
                };
            }
        }

        public class Response
        {
            public List<FiestaUser> Friends { get; set; }
        }
    }
}
