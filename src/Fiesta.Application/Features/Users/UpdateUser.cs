using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Common.Validators;
using Fiesta.Application.Utils;
using FluentValidation;
using MediatR;

namespace Fiesta.Application.Features.Users
{
    public class UpdateUser
    {
        public class Command : IRequest<Response>
        {
            public string UserId { get; set; }
            public Optional<string> FirstName { get; set; }
            public Optional<string> LastName { get; set; }
            public Optional<string> Username { get; set; }
            public Optional<string> Bio { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IFiestaDbContext _db;
            private readonly IAuthService _authService;

            public Handler(IFiestaDbContext db, IAuthService authService)
            {
                _db = db;
                _authService = authService;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var fiestaUser = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.UserId);

                if (request.FirstName.HasValue)
                    fiestaUser.FirstName = request.FirstName.Value;
                if (request.LastName.HasValue)
                    fiestaUser.LastName = request.LastName.Value;
                if (request.Username.HasValue)
                {
                    var usernameResult = await _authService.UpdateUsername(request.UserId, request.Username.Value, cancellationToken);

                    if (usernameResult.Failed)
                        throw new BadRequestException(usernameResult.Errors);

                    fiestaUser.UpdateUsername(request.Username.Value);
                }
                if (request.Bio.HasValue)
                    fiestaUser.Bio = request.Bio.Value;

                await _db.SaveChangesAsync(cancellationToken);

                return new Response
                {
                    Id = fiestaUser.Id,
                    FirstName = fiestaUser.FirstName,
                    LastName = fiestaUser.LastName,
                    FullName = fiestaUser.FullName,
                    Username = fiestaUser.Username,
                    Bio = fiestaUser.Bio
                };
            }
        }

        public class Validator : AbstractValidatorPlus<Command>
        {
            private readonly IAuthService _authService;

            public Validator(IAuthService authService)
            {
                _authService = authService;

                RuleForOptional(x => x.FirstName)
                   .NotEmpty().WithErrorCode(ErrorCodes.Required)
                   .MinimumLength(2).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 2 })
                   .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });

                RuleForOptional(x => x.LastName)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(2).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 2 })
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });

                RuleForOptional(x => x.Username)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(2).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 2 })
                    .MaximumLength(15).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 15 })
                    .Must(ContainAllowedCharacters).WithErrorCode(ErrorCodes.CanContainLettersNumberOrSpecialCharacters).WithState(_ => new { SpecialCharacters = "-._@+#" })
                    .MustAsync(BeUnique).WithErrorCode(ErrorCodes.AlreadyExists);

                RuleForOptional(x => x.Bio)
                   .MaximumLength(500).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 500 });
            }

            private async Task<bool> BeUnique(string username, CancellationToken cancellationToken)
            {
                return await _authService.IsUsernameUnique(username, cancellationToken);
            }

            private bool ContainAllowedCharacters(string username)
            {
                return username.All(x => Helpers.UsernameAllowedCharacters.Contains(x));
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => Task.FromResult(currentUserService.IsResourceOwnerOrAdmin(request.UserId));
        }

        public class Response
        {
            public string Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string FullName { get; set; }

            public string Username { get; set; }

            public string Bio { get; set; }
        }
    }
}
