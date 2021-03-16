using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;

namespace Fiesta.Application.Common.Behaviours.Authorization
{
    public interface IAuthorizationCheck<TRequest>
    {
        public Task<bool> IsAuthorized(TRequest request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken);
    }
}
