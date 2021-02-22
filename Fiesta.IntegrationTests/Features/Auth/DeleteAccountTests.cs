using System.Threading.Tasks;
using Xunit;

namespace Fiesta.IntegrationTests.Features.Auth
{
    [Collection(nameof(FiestaAppFactory))]
    public class DeleteAccountTests : WebAppTestBase
    {
        public DeleteAccountTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidPassword_WhenDeleteAccountWithPassword_AccountIsDeleted()
        {

        }

        [Fact]
        public async Task GivenInvalidPassword_WhenDeleteAccountWithPassword_BadRequestIsReturned()
        {

        }

        [Fact]
        public async Task GivenValidCode_WhenDeleteAccountWithGoogle_AccountIsDeleted()
        {

        }

        [Fact]
        public async Task GivenInvalidCode_WhenDeleteAccountWithGoogle_BadRequestIsReturned()
        {

        }
    }
}
