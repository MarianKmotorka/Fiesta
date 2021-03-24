using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users
{
    public class HardDeleteUsers
    {
        public class Command : IRequest
        {
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
                var deletedUsers = await _db.FiestaUsers.IgnoreQueryFilters().Where(x => x.IsDeleted).ToListAsync(cancellationToken);
                _db.FiestaUsers.RemoveRange(deletedUsers);

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
