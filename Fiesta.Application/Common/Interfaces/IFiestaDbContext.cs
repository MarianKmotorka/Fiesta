using System;
using Fiesta.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IFiestaDbContext : IDisposable
    {
        DbSet<FiestaUser> FiestaUsers { get; }
    }
}
