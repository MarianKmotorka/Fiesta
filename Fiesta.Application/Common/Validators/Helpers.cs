using FluentValidation;
using System;

namespace Fiesta.Application.Common.Validators
{
    public static class Helpers
    {
        public static IRuleBuilderOptions<T, TProperty> HasEnumValidValue<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : struct, Enum
        {
            return ruleBuilder.Must(HasEnumValidValue);
        }

        private static bool HasEnumValidValue<T>(this T enumValue) where T : struct, Enum
        {
            return Enum.IsDefined<T>(enumValue);
        }
    }
}
