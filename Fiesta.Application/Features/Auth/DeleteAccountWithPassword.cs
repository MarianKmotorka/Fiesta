using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using MediatR;

namespace Fiesta.Application.Features.Auth
{
    public class DeleteAccountWithPassword
    {
        public class Command : IRequest
        {
            public string UserId { get; set; }
            public string Password { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IAuthService _authService;

            public Handler(IAuthService authService)
            {
                _authService = authService;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                await _authService.DeleteAccountWithPassword(request.UserId, request.Password, cancellationToken);
                return Unit.Value;
            }
        }
    }
}
