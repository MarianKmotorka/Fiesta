using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Messaging.Email.Models;
using Fiesta.Application.Utils;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Auth
{
    public class SendVerificationEmail
    {
        public class Command : IRequest
        {
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IAuthService _authService;
            private readonly IEmailService _emailService;
            private readonly IFiestaDbContext _fiestaDbContext;

            public Handler(IAuthService authService, IEmailService emailService, IFiestaDbContext fiestaDbContext)
            {
                _authService = authService;
                _emailService = emailService;
                _fiestaDbContext = fiestaDbContext;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _fiestaDbContext.FiestaUsers.SingleOrNotFoundAsync(x => x.Email == request.Email, cancellationToken);
                var code = await _authService.GetEmailVerificationCode(user.Email, cancellationToken);
                var result = await _emailService.SendVerificationEmail(user.Email, new VerificationModel(user.FirstName, code), cancellationToken);

                if (!result.Successful)
                    throw new BadRequestException(result.ErrorMessages);

                return Unit.Value;
            }
        }

    }
}
