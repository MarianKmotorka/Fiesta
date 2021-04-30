using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events.Comments
{
    public class DeleteComment
    {
        public class Command : IRequest
        {
            public string CommentId { get; set; }

            public string EventId { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var toDelete = await _db.EventComments.Where(x => x.Id == request.CommentId || x.ParentId == request.CommentId).ToListAsync(cancellationToken);
                _db.EventComments.RemoveRange(toDelete);

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
            {
                var comment = await db.EventComments.SingleOrNotFoundAsync(x => x.Id == request.CommentId && x.EventId == request.EventId, cancellationToken);
                return currentUserService.IsResourceOwnerOrAdmin(comment.SenderId);
            }
        }
    }
}
