using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users
{
    public class UsersSelector
    {
        public class Query : IRequest<List<ResponseDto>>
        {
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
                var query = _db.FiestaUsers.AsQueryable();

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(x => x.Username.Contains(request.Search)
                                          || x.FirstName.Contains(request.Search)
                                          || x.LastName.Contains(request.Search));


                return await query.Select(x => new ResponseDto
                {
                    Id = x.Id,
                    Username = x.Username,
                    FullName = x.FullName,
                    PictureUrl = x.PictureUrl
                })
                .OrderBy(x => x.Username)
                .Take(25)
                .ToListAsync(cancellationToken);

            }
        }

        public class ResponseDto
        {
            public string Id { get; set; }

            public string Username { get; set; }

            public string FullName { get; set; }

            public string PictureUrl { get; set; }
        }
    }
}
