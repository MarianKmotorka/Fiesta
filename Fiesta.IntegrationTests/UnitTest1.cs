using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fiesta.IntegrationTests
{
    public class UnitTest1 : WebAppTestBase
    {

        [Fact]
        public async Task ThisTestPasses()
        {
            var users = AssertDb.FiestaUsers.ToList();
        }

        [Fact]
        public async Task ThisTestPassesToo()
        {
            var users = AssertDb.FiestaUsers.ToList();

        }

        [Fact]
        public async Task ThisTestPassesTooo()
        {
            var users = AssertDb.FiestaUsers.ToList();

        }
    }
}
