using System.Threading.Tasks;
using Fiesta.Application.Features.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Users
{
    [Collection(nameof(TestCollection))]
    public class HardDeleteUsersTests : WebAppTestBase
    {
        private HardDeleteUsers.Handler _sut;

        public HardDeleteUsersTests(FiestaAppFactory factory) : base(factory)
        {
            _sut = new HardDeleteUsers.Handler(ActDb);
        }

        [Fact]
        public async Task GivenSomeUsersMarkedAsDeleted_WhenHandlerIsCalled_OnlyMarkedUsersAreDeleted()
        {
            var (_, deletedUser) = ArrangeDb.SeedBasicUser(x => x.SetDeleted());
            var (_, notDeletedUser) = ArrangeDb.SeedBasicUser();
            await ArrangeDb.SaveChangesAsync();

            await _sut.Handle(new HardDeleteUsers.Command(), default);

            var userDb = await AssertDb.FiestaUsers.IgnoreQueryFilters().SingleAsync(x => x.Id == notDeletedUser.Id);
            userDb.IsDeleted.Should().BeFalse();

            var deletedUserExists = await AssertDb.FiestaUsers.IgnoreQueryFilters().AnyAsync(x => x.Id == deletedUser.Id);
            deletedUserExists.Should().BeFalse();
        }
    }
}
