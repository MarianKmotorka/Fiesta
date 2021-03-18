using FluentValidation;
using FluentValidation.Internal;

namespace Fiesta.Application.Common.Validators
{
    public interface IRuleBuilderInitialOptional<T, TElement>
       : IRuleBuilder<T, TElement>, IConfigurable<OptionalPropertyRule<T, TElement>, IRuleBuilderInitialOptional<T, TElement>>, IRuleBuilderInitial<T, TElement>
    {
    }
}
