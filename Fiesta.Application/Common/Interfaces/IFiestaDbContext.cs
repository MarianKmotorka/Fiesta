using Fiesta.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IFiestaDbContext : IDisposable
    {
        DbSet<FiestaUser> FiestaUsers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
