using System.Threading.Tasks;
using Fiesta.Application.Features.Users;
using Fiesta.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fiesta.IntegrationTests.Features.Users
{
    [Collection(nameof(FiestaAppFactory))]
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
            var deletedUser = ArrangeDb.FiestaUsers.Add(new FiestaUser("deleted@email.com", "Deleted") { IsDeleted = true }).Entity;
            var notDeletedUser = ArrangeDb.FiestaUsers.Add(new FiestaUser("NOTdeleted@email.com", "NotDeleted")).Entity;
            await ArrangeDb.SaveChangesAsync();

            await _sut.Handle(new HardDeleteUsers.Command(), default);

            var userDb = await AssertDb.FiestaUsers.IgnoreQueryFilters().SingleAsync(x => x.Id == notDeletedUser.Id);
            userDb.IsDeleted.Should().BeFalse();

            var deletedUserExists = await AssertDb.FiestaUsers.IgnoreQueryFilters().AnyAsync(x => x.Id == deletedUser.Id);
            deletedUserExists.Should().BeFalse();
        }
    }
}
