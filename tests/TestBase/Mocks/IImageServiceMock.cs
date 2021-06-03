using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Models;
using NSubstitute;

namespace TestBase.Mocks
{
    public static class IImageServiceMock
    {
        public static IImageService Mock { get; }

        static IImageServiceMock()
        {
            Mock = Substitute.For<IImageService>();

            Mock.Delete(default, default)
                .ReturnsForAnyArgs(Task.FromResult(Result.Success()));
        }
    }
}
