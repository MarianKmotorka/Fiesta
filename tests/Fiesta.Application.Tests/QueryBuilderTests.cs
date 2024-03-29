using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiesta.Application.Common.Queries;
using Fiesta.Domain.Entities.Users;
using FluentAssertions;
using TestBase;
using Xunit;

namespace Fiesta.Infrastracture.UnitTests
{
    public class QueryBuilderTests : DbTestBase
    {
        private FiestaUser _adeleVance;
        private FiestaUser _adeleJohnes;
        private FiestaUser _johnTravolta;
        private FiestaUser _harryClansy;
        private FiestaUser _thomasClansy;

        public QueryBuilderTests()
        {
            _adeleVance = new FiestaUser("adele@GMAIL.com", "Adolfina1")
            {
                FirstName = "Adele",
                LastName = "Vance",
                CreatedOnUtc = DateTime.MinValue
            };

            _adeleJohnes = new FiestaUser("adele@centrum.com", "Adolfina2")
            {
                FirstName = "Adele",
                LastName = "Johnes",
                CreatedOnUtc = DateTime.MinValue
            };

            _johnTravolta = new FiestaUser("john@azet.com", "Johnny")
            {
                FirstName = "John",
                LastName = "Travolta",
                CreatedOnUtc = DateTime.MaxValue
            };

            _harryClansy = new FiestaUser("harry@gmail.com", "Harietta")
            {
                FirstName = "Harold",
                LastName = "Clansy",
                CreatedOnUtc = DateTime.MaxValue
            };

            _thomasClansy = new FiestaUser("thomas@bing.com", "Tommy")
            {
                FirstName = "Thomas",
                LastName = "Clansy",
                CreatedOnUtc = DateTime.MinValue
            };

            ArrangeDb.FiestaUsers.AddRange(_johnTravolta, _adeleVance, _adeleJohnes, _harryClansy, _thomasClansy);
            ArrangeDb.SaveChanges();
        }

        [Fact]
        public async Task WhenFiltersAreSpecified_CorrectQueryResponseIsReturned()
        {
            var document = new QueryDocument
            {
                Filters = new List<Filter> { new Filter("email", Operation.Contains, "gmail") }
            };

            var result = await AssertDb.FiestaUsers.BuildResponse(document, default);
            result.Entries.All(x => x.Email.ToLower().Contains("gmail")).Should().BeTrue();
        }

        [Fact]
        public async Task WhenMultipleSortsAreSpecified_ResultIsSortedByBySpecifiedSortsRespectively()
        {
            var document = new QueryDocument
            {
                Sorts = new List<Sort> { new Sort("firstName", SortType.Desc), new Sort("lastName", SortType.Asc) }
            };

            var result = await AssertDb.FiestaUsers.BuildResponse(document, default);

            result.Entries.Select(x => x.Id)
                .ToArray()
                .Should()
                .BeEquivalentTo(
                    new[] { _thomasClansy.Id, _johnTravolta.Id, _harryClansy.Id, _adeleJohnes.Id, _adeleVance.Id },
                    options => options.WithStrictOrdering()
                    );
        }

        [Fact]
        public async Task WhenPageAndPageSizeAreSpecified_CorrectQueryResponseIsReturned()
        {
            var document = new QueryDocument
            {
                Page = 1,
                PageSize = 2,
                Sorts = new List<Sort> { new Sort("email", SortType.Asc) }
            };

            var result = await AssertDb.FiestaUsers.BuildResponse(document, default);

            result.Page.Should().Be(1);
            result.PageSize.Should().Be(2);
            result.TotalEntries.Should().Be(5);
            result.TotalPages.Should().Be(3);
            result.Entries.Should().HaveCount(2);
            result.Entries.Select(x => x.Id).ToArray().Should().BeEquivalentTo(new[] { _johnTravolta.Id, _harryClansy.Id });
        }

        [Fact]
        public async Task WhenPageSizeAndFilterAreSpecified_CorrectQueryResponseIsReturned()
        {
            var document = new QueryDocument
            {
                Page = 0,
                PageSize = 20,
                Filters = new List<Filter> { new Filter("createdOnUtc", Operation.GreaterThan, new DateTime(2000, 1, 1)) }
            };

            var result = await AssertDb.FiestaUsers.BuildResponse(document, default);

            result.Page.Should().Be(0);
            result.PageSize.Should().Be(20);
            result.TotalEntries.Should().Be(2);
            result.TotalPages.Should().Be(1);
            result.Entries.Should().HaveCount(2);

            result.Entries.Select(x => x.Id)
                .ToArray()
                .Should()
                .BeEquivalentTo(new[] { _johnTravolta.Id, _harryClansy.Id });
        }
    }
}
