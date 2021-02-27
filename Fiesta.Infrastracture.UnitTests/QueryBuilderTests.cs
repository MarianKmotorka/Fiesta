using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiesta.Application.Common.Queries;
using Fiesta.Domain.Entities.Users;
using Fiesta.Infrastracture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Fiesta.Infrastracture.UnitTests
{
    public class QueryBuilderTests
    {
        [Fact]
        public async Task Test1()
        {
            var db = CreateDb();
            var adele = new FiestaUser("adele@gmail.com");
            var bob = new FiestaUser("bob@centrum.com");
            bob.IsDeleted = true;
            var john = new FiestaUser("john@gmail.com");
            var harry = new FiestaUser("harry@gmail.com");
            harry.IsDeleted = true;
            var kate = new FiestaUser("kate@gmail.com");
            db.FiestaUsers.AddRange(john, kate, adele, bob, harry);
            await db.SaveChangesAsync();

            var document = new QueryDocument
            {
                //Filters = new List<Filter> { new Filter("email", OperationEnum.Contains, "gmail") },
                Sorts = new List<Sort> { new Sort("isdeleted", OrderType.Desc), new Sort("email", OrderType.Desc) }
            };

            var users = await db.FiestaUsers
                .OrderBy(x => x.Email)
                .ThenBy(x => x.IsDeleted)
                .ToListAsync();

            var users2 = await db.FiestaUsers
                .OrderBy(x => x.Email)
                .OrderBy(x => x.IsDeleted)
                .ToListAsync();

            var users3 = await db.FiestaUsers
                .BuildQuery(document)
                .ToListAsync();
        }

        private static FiestaDbContext CreateDb()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FiestaDbContext>().UseInMemoryDatabase(FiestaDbContext.TestDbName);
            return new FiestaDbContext(optionsBuilder.Options, Substitute.For<IMediator>());
        }
    }
}
