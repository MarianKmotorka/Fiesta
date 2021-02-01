using Xunit;

namespace Fiesta.IntegrationTests
{
    public class UnitTest1
    {
        [Fact]
        public void ThisTestPasses()
        {

        }

        [Fact]
        public void ThisTestFails()
        {
            Assert.False(true);
        }
    }
}
