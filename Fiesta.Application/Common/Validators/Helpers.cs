using Fiesta.Application.Common.Constants;
using FluentValidation;
using System;

namespace Fiesta.Application.Common.Validators
{
    public static class Helpers
    {
        public static IRuleBuilderOptions<T, TProperty> HasEnumValidValue<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : struct, Enum
        {
            return ruleBuilder.Must(x => Enum.IsDefined(x)).WithErrorCode(ErrorCodes.InvalidEnumValue);
        }
    }
}
