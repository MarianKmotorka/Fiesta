using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Constants;
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
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var fiestaUser = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.UserId);

                if (request.FirstName.HasValue)
                    fiestaUser.FirstName = request.FirstName.Value;
                if (request.LastName.HasValue)
                    fiestaUser.LastName = request.LastName.Value;

                await _db.SaveChangesAsync(cancellationToken);
                return new Response
                {
                    Id = fiestaUser.Id,
                    FirstName = fiestaUser.FirstName,
                    LastName = fiestaUser.LastName,
                    FullName = fiestaUser.FullName
                };
            }
        }

        public class Validator : AbstractValidatorPlus<Command>
        {
            public Validator()
            {
                RuleForOptional(x => x.FirstName)
                   .NotEmpty().WithErrorCode(ErrorCodes.Required)
                   .MinimumLength(2).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 2 })
                   .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });

                RuleForOptional(x => x.LastName)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(2).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 2 })
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });
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
        }
    }
}
