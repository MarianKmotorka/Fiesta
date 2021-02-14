using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Fiesta.Application.Features.Auth
{
    public class ResetPassword
    {
        public class Command : IRequest
        {
            public string Email { get; set; }

            public string Token { get; set; }

            public string NewPassword { get; set; }
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
                await _authService.ResetPassword(request.Email, request.Token, request.NewPassword, cancellationToken);
                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(6).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 6 })
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });
            }
        }
    }
}
