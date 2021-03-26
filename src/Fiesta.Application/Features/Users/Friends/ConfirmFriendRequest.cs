using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fiesta.Application.Features.Users.Friends
{
    public class ConfirmFriendRequest
    {
        public class Command : IRequest<Unit>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            public string FriendId { get; set; }
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
                var friendRequest = await _db.FriendRequests.SingleOrDefaultAsync(x => x.FromId == request.FriendId && x.ToId == request.UserId, cancellationToken);

                if (friendRequest is null)
                    throw new BadRequestException("Friend request does not exist");

                _db.FriendRequests.Remove(friendRequest);

                var fiestaUser = await _db.FiestaUsers.SingleOrNotFoundAsync(x => x.Id == request.UserId, cancellationToken);
                var fiestaFriend = await _db.FiestaUsers.SingleOrNotFoundAsync(x => x.Id == request.FriendId, cancellationToken);

                fiestaUser.AddFriend(fiestaFriend);
                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly IFiestaDbContext _db;

            public Validator(IFiestaDbContext db)
            {
                _db = db;

                RuleFor(x => x.FriendId)
                   .NotEmpty().WithErrorCode(ErrorCodes.Required)
                   .MustAsync(Exist).WithErrorCode(ErrorCodes.DoesNotExist);
            }

            private async Task<bool> Exist(string userId, CancellationToken cancellationToken)
            {
                return await _db.FiestaUsers.AnyAsync(x => x.Id == userId, cancellationToken);
            }
        }
    }
}
