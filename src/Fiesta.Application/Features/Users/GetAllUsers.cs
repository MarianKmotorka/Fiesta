using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users
{
    public class GetAllUsers
    {
        public class Query : IRequest<QueryResponse<ResponseDto>>
        {
            public string Search { get; set; }

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
                var query = _db.FiestaUsers.AsNoTracking().IgnoreQueryFilters();

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(x => x.Username.Contains(request.Search) || (x.FirstName + " " + x.LastName).Contains(request.Search));

                var users = await query.Select(x => new ResponseDto
                {
                    Id = x.Id,
                    Username = x.Username,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PictureUrl = x.PictureUrl,
                    IsDeleted = x.IsDeleted,
                })
                .OrderBy(x => x.Username)
                .BuildResponse(request.QueryDocument, cancellationToken);

                return users;
            }
        }

        public class ResponseDto : UserDto
        {
            public bool EmailConfirmed { get; set; }

            public FiestaRoleEnum Role { get; set; }

            public bool IsDeleted { get; set; }
        }
    }
}

