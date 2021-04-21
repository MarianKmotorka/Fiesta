using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class GetEventInvitations
    {
        public class Query : IRequest<QueryResponse<ResponseDto>>
        {
            [JsonIgnore]
            public string EventId { get; set; }

            public QueryDocument QueryDocument { get; set; } = new();

            [JsonIgnore]
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<Query, QueryResponse<ResponseDto>>
        {
            private IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResponse<ResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _db.EventInvitations.AsNoTracking().Where(x => x.EventId == request.EventId);

                if (!string.IsNullOrEmpty(request.Search))
                    query = query
                        .Where(x => x.Invitee.Username.Contains(request.Search) || (x.Invitee.FirstName + " " + x.Invitee.LastName).Contains(request.Search));

                return await query
                    .OrderBy(x => x.Invitee.Username)
                    .Select(x => new ResponseDto
                    {
                        Invitee = new UserDto
                        {
                            Id = x.InviteeId,
                            Username = x.Invitee.Username,
                            PictureUrl = x.Invitee.PictureUrl,
                            FirstName = x.Invitee.FirstName,
                            LastName = x.Invitee.LastName,
                        },
                        Inviter = new UserDto
                        {
                            Id = x.InviterId,
                            Username = x.Inviter.Username,
                            PictureUrl = x.Inviter.PictureUrl,
                            FirstName = x.Invitee.FirstName,
                            LastName = x.Invitee.LastName,
                        },
                        CreatedAtUtc = x.CreatedAtUtc
                    })
                    .BuildResponse(request.QueryDocument, cancellationToken);
            }
        }

        public class ResponseDto
        {
            public UserDto Invitee { get; set; }

            public UserDto Inviter { get; set; }

            public DateTime CreatedAtUtc { get; set; }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public async Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => await Helpers.IsOrganizerOrAdmin(request.EventId, db, currentUserService, cancellationToken);
        }
    }
}
