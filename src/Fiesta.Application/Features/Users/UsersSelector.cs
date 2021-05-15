using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users
{
    public class UsersSelector
    {
        public class Query : IRequest<List<UserDto>>
        {
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
                var query = _db.FiestaUsers.AsNoTracking();

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(x => x.Username.Contains(request.Search) || (x.FirstName + " " + x.LastName).Contains(request.Search));

                return await query.Select(x => new UserDto
                {
                    Id = x.Id,
                    Username = x.Username,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PictureUrl = x.PictureUrl
                })
                .OrderBy(x => x.Username)
                .Take(25)
                .ToListAsync(cancellationToken);
            }
        }
    }
}
