using System;
using Fiesta.Infrastracture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace TestBase
{
    public class DbTestBase : IDisposable
    {
        public FiestaDbContext ArrangeDb { get; }

        public FiestaDbContext ActDb { get; }

        public FiestaDbContext AssertDb { get; }

        public DbTestBase()
        {
            ArrangeDb = CreateDb();
            ActDb = CreateDb();
            AssertDb = CreateDb();
        }

        private static FiestaDbContext CreateDb()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FiestaDbContext>().UseInMemoryDatabase(FiestaDbContext.TestDbName);
            return new FiestaDbContext(optionsBuilder.Options, Substitute.For<IMediator>());
        }

        public void Dispose()
        {
            ArrangeDb.Database.EnsureDeleted();
            ArrangeDb.Dispose();
            ActDb.Dispose();
            AssertDb.Dispose();
        }
    }
}
