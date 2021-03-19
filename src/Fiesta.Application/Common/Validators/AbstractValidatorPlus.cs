using System;
using System.Linq.Expressions;
using Fiesta.Application.Common.Models;
using FluentValidation;

namespace Fiesta.Application.Common.Validators
{
    public abstract class AbstractValidatorPlus<T> : AbstractValidator<T>
    {
        protected IRuleBuilderInitialOptional<T, TProperty> RuleForOptional<TProperty>(Expression<Func<T, Optional<TProperty>>> expression)
        {
            var rule = OptionalPropertyRule<T, TProperty>.Create(expression, () => CascadeMode);
            AddRule(rule);
            var ruleBuilder = new OptionalRuleBuilder<T, TProperty>(rule, this);
            return ruleBuilder;
        }
    }
}
