using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Models.Emails;
using Fiesta.Domain.Entities.Users;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Auth
{
    public class RegisterWithEmailAndPassword
    {
        public class Command : IRequest
        {
            public string Email { get; set; }

            public string Password { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IAuthService _authService;
            private readonly IMediator _mediator;
            private readonly IEmailService _emailService;

            public Handler(IAuthService authService, IMediator mediator, IEmailService emailService)
            {
                _authService = authService;
                _mediator = mediator;
                _emailService = emailService;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = await _authService.Register(request, cancellationToken);

                await _mediator.Publish(new AuthUserCreatedEvent(userId, request.Email)
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                }, cancellationToken);

                var emailCode = await _authService.GetEmailVerificationCode(request.Email, cancellationToken);

                await _emailService.SendVerificationEmail(request.Email, new VerificationEmailTemplateModel(request.FirstName, emailCode), cancellationToken);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly IFiestaDbContext _db;

            public Validator(IFiestaDbContext db)
            {
                _db = db;

                RuleFor(x => x.Email)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .EmailAddress().WithErrorCode(ErrorCodes.InvalidEmailAddress)
                    .MustAsync(BeUnique).WithErrorCode(ErrorCodes.MustBeUnique);

                RuleFor(x => x.Password)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(6).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 6 })
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });

                RuleFor(x => x.FirstName)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(2).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 2 })
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });

                RuleFor(x => x.LastName)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(2).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 2 })
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });
            }

            private async Task<bool> BeUnique(string email, CancellationToken cancellationToken)
            {
                return await _db.FiestaUsers.AllAsync(x => x.Email != email, cancellationToken);
            }
        }
    }
}
