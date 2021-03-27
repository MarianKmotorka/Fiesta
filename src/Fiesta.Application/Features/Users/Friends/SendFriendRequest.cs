using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fiesta.Application.Features.Users.Friends
{
    public class SendFriendRequest
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
                var fiestaUser = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.UserId);
                var fiestaFriend = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.FriendId);

                fiestaUser.SendFriendRequest(fiestaFriend);
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
                   .MustAsync(Exist).WithErrorCode(ErrorCodes.DoesNotExist)
                   .MustAsync(NotBeFriend).WithErrorCode(ErrorCodes.AlreadyFriends)
                   .Must(NotBeSamePerson).WithErrorCode(ErrorCodes.SenderAndReceiverIdentical);
            }

            private async Task<bool> Exist(string userId, CancellationToken cancellationToken)
            {
                return await _db.FiestaUsers.AnyAsync(x => x.Id == userId, cancellationToken);
            }

            private async Task<bool> NotBeFriend(Command command, string _, CancellationToken cancellationToken)
            {
                return !await _db.UserFriends.AnyAsync(x => x.UserId == command.UserId && x.FriendId == command.FriendId, cancellationToken);
            }

            private bool NotBeSamePerson(Command command, string friendId)
            {
                return command.UserId != friendId;
            }
        }
    }
}
