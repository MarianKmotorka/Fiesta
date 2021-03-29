using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;

namespace Fiesta.Application.Features.Events
{
    public class DeleteEventJoinRequest
    {
        public class Command : IRequest
        {
            public string CurrentUserId { get; set; }

            public string EventId { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var joinRequest = await _db.EventJoinRequests
                    .SingleOrNotFoundAsync(x => x.EventId == request.EventId && x.InterestedUserId == request.CurrentUserId, cancellationToken);

                _db.EventJoinRequests.Remove(joinRequest);
                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
