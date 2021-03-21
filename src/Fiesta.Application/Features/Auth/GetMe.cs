using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;

namespace Fiesta.Application.Features.Auth
{
    public class GetMe
    {
        public class Query : IRequest<Response>
        {
            [JsonIgnore]
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IFiestaDbContext _db;
            private readonly IAuthService _authService;

            public Handler(IFiestaDbContext db, IAuthService authService)
            {
                _db = db;
                _authService = authService;
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
                    FullName = user.FullName,
                    Username = user.Username,
                    PictureUrl = user.PictureUrl,
                    Role = await _authService.GetRole(user.Id, cancellationToken),
                    GoogleEmail = await _authService.GetGoogleEmail(user.Id, cancellationToken),
                    AuthProvider = await _authService.GetAuthProvider(user.Id, cancellationToken),
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

            public string Email { get; set; }

            public string GoogleEmail { get; set; }

            public string PictureUrl { get; set; }

            public AuthProviderEnum AuthProvider { get; set; }

            public FiestaRoleEnum Role { get; set; }
        }
    }
}
