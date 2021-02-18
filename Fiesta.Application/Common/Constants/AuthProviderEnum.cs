using System;

namespace Fiesta.Application.Common.Constants
{
    [Flags]
    public enum AuthProviderEnum
    {
        None = 0,
        Google = 1,
        EmailAndPassword = 2,
        Facebook = 4
    }
}
