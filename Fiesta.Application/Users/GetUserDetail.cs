﻿using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;

namespace Fiesta.Application.Users
{
    public class GetUserDetail
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
                var user = await _db.FiestaUsers.SingleOrNotFoundAsync(x => x.Id == request.Id, cancellationToken);
                return new Response
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PictureUrl = user.PictureUrl
                };
            }
        }

        public class Response
        {
            public string Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }

            public string PictureUrl { get; set; }
        }
    }
}
