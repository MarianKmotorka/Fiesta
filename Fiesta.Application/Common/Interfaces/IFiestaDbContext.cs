using System;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IFiestaDbContext : IDisposable
    {
        DbSet<FiestaUser> FiestaUsers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
