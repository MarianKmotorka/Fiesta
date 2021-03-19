using System;
using FluentValidation;
using FluentValidation.Internal;

namespace Fiesta.Application.Common.Validators
{
    public class OptionalRuleBuilder<T, TProperty> : RuleBuilder<T, TProperty>, IRuleBuilderInitialOptional<T, TProperty>
    {
        public OptionalRuleBuilder(PropertyRule rule, IValidator<T> parent) : base(rule, parent)
        {
        }

        public IRuleBuilderInitialOptional<T, TProperty> Configure(Action<OptionalPropertyRule<T, TProperty>> configurator)
        {
            configurator((OptionalPropertyRule<T, TProperty>)Rule);
            return this;
        }
    }
}
