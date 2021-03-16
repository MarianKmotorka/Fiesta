using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Models.Emails;
using Fiesta.Application.Utils;
using MediatR;

namespace Fiesta.Application.Features.Auth
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

                var codeResult = await _authService.GetEmailVerificationCode(user.Email, cancellationToken);
                if (codeResult.Failed)
                    throw new BadRequestException(codeResult.Errors);

                var emailResult = await _emailService.SendVerificationEmail(
                    user.Email,
                    new VerificationEmailTemplateModel(user.FirstName, codeResult.Data),
                    cancellationToken);

                if (!emailResult.Successful)
                    throw new BadRequestException(emailResult.ErrorMessages);

                return Unit.Value;
            }
        }

    }
}
