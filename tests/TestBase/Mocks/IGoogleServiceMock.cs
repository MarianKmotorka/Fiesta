using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Features.Auth.CommonDtos;
using NSubstitute;
using TestBase.Assets;

namespace TestBase.Mocks
{
    public static class IGoogleServiceMock
    {
        public static IGoogleService Mock { get; }

        static IGoogleServiceMock()
        {
            Mock = Substitute.For<IGoogleService>();

            Mock
                .GetUserInfoModelForLogin(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(x => x.Args()[0].ToString() == "validCode"
                    ? Task.FromResult(Result.Success(GoogleAssets.JohnyUserInfoModel))
                    : Task.FromResult(Result<GoogleUserInfoModel>.Failure(ErrorCodes.InvalidCode)));
            Mock
                .GetUserInfoModelForConnectAccount(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(x => x.Args()[0].ToString() == "validCode"
                    ? Task.FromResult(Result.Success(GoogleAssets.JohnyUserInfoModel))
                    : Task.FromResult(Result<GoogleUserInfoModel>.Failure(ErrorCodes.InvalidCode)));
            Mock
                .GetUserInfoModelForDeleteAccount(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(x => x.Args()[0].ToString() == "validCode"
                    ? Task.FromResult(Result.Success(GoogleAssets.JohnyUserInfoModel))
                    : Task.FromResult(Result<GoogleUserInfoModel>.Failure(ErrorCodes.InvalidCode)));
        }
    }
}
