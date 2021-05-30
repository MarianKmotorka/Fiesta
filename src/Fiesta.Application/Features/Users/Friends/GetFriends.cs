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
        public class Query : IRequest<List<UserDto>>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            [JsonIgnore]
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<UserDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<List<UserDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var usersQuery = _db.UserFriends.AsNoTracking().Where(x => x.UserId == request.UserId)
                   .Select(x => new UserDto
                   {
                       Id = x.Friend.Id,
                       Username = x.Friend.Username,
                       FirstName = x.Friend.FirstName,
                       LastName = x.Friend.LastName,
                       PictureUrl = x.Friend.PictureUrl
                   });

                if (!string.IsNullOrEmpty(request.Search))
                    usersQuery = usersQuery.Where(x => (x.FirstName + " " + x.LastName).Contains(request.Search));

                return await usersQuery.Take(25).ToListAsync(cancellationToken);
            }
        }
    }
}
