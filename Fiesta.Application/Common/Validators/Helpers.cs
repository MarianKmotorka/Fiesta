using System;

namespace Fiesta.Application.Common.Validators
{
    public static class Helpers
    {
        public static bool HasEnumValidValue<T>(this T enumValue) where T : struct, Enum
        {
            return Enum.IsDefined<T>(enumValue);
        }
    }
}
