using Fiesta.IntegrationTests;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Users
{
    [Collection(nameof(FiestaAppFactory))]
    public class UpdateUserTests : WebAppTestBase
    {
        public UpdateUserTests(FiestaAppFactory factory) : base(factory)
        {
        }
    }
}
