using System.Net;
using System.Threading.Tasks;
using Fiesta.Application.Features.Users;
using FluentAssertions;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Users
{
    [Collection(nameof(TestCollection))]
    public class DeleteProfilePictureTests : WebAppTestBase
    {
        private HardDeleteUsers.Handler _sut;

        public DeleteProfilePictureTests(FiestaAppFactory factory) : base(factory)
        {
            _sut = new HardDeleteUsers.Handler(ActDb);
        }

        [Fact]
        public async Task GivenUserWithProfilePicture_WhenDeletingOwnPicture_PictureIsDeleted()
        {
            var (authUser, _) = ArrangeDb.SeedBasicUser(user => user.PictureUrl = "pictureUrl");
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(authUser);
            var response = await client.DeleteAsync($"/api/users/{authUser.Id}/profile-picture");

            response.EnsureSuccessStatusCode();
            var fiestaUserDb = await AssertDb.FiestaUsers.FindAsync(authUser.Id);
            fiestaUserDb.PictureUrl.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GivenUserWithProfilePicture_WhenDeletingPictureOfOtherUserButIsAdmin_PictureIsDeleted()
        {
            var (admin, _) = ArrangeDb.SeedAdmin();
            var (otherUser, _) = ArrangeDb.SeedBasicUser(user => user.PictureUrl = "pictureUrl");
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(admin);
            var response = await client.DeleteAsync($"/api/users/{otherUser.Id}/profile-picture");

            response.EnsureSuccessStatusCode();
            var fiestaUserDb = await AssertDb.FiestaUsers.FindAsync(otherUser.Id);
            fiestaUserDb.PictureUrl.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GivenUserWithProfilePicture_WhenDeletingPictureOfOtherUser_ForbiddenIsReturned()
        {
            var (authUser, _) = ArrangeDb.SeedBasicUser();
            var (otherUser, _) = ArrangeDb.SeedBasicUser(null, authUser => authUser.Email = "other@user.com");
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(authUser);
            var response = await client.DeleteAsync($"/api/users/{otherUser.Id}/profile-picture");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}