using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using NSubstitute;

namespace TestBase.Mocks
{
    public static class IEmailServiceMock
    {
        public static IEmailService Mock { get; }

        static IEmailServiceMock()
        {
            Mock = Substitute.For<IEmailService>();

            Mock
                .SendVerificationEmail(default, default, default)
                .ReturnsForAnyArgs(Task.FromResult(new FluentEmail.Core.Models.SendResponse()));
        }
    }
}
