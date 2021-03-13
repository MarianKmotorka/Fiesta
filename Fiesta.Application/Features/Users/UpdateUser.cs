using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Features.Users
{
    public class UpdateUser
    {
        public class Command : IRequest<Unit>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var fiestaUser = await _db.FiestaUsers.FindAsync(new[] { request.UserId }, cancellationToken);

                fiestaUser.FirstName = request.FirstName;
                fiestaUser.LastName = request.LastName;

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.FirstName)
                   .NotEmpty().WithErrorCode(ErrorCodes.Required)
                   .MinimumLength(2).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 2 })
                   .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });

                RuleFor(x => x.LastName)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(2).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 2 })
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });
            }
        }
    }
}
