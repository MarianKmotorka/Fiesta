using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Models;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace Fiesta.Application.Common.Validators
{
    public class OptionalPropertyRule<T, TProperty> : PropertyRule
    {
        public OptionalPropertyRule(MemberInfo member, Func<object, object> propertyFunc, LambdaExpression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type containerType)
            : base(member, propertyFunc, expression, cascadeModeThunk, typeToValidate, containerType)
        {
        }

        public static OptionalPropertyRule<T, TProperty> Create(Expression<Func<T, Optional<TProperty>>> expression, Func<CascadeMode> cascadeModeThunk)
        {
            var member = expression.GetMember();
            var compiled = expression.Compile();

            return new OptionalPropertyRule<T, TProperty>(member, compiled.CoerceToNonGeneric(), expression, cascadeModeThunk, typeof(TProperty), typeof(T));
        }

        protected override IEnumerable<ValidationFailure> InvokePropertyValidator(IValidationContext context, IPropertyValidator validator, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                propertyName = InferPropertyName(Expression);

            PropertyValidatorContext propertyContext;
            // TODO: For FV10 this will come as a parameter rather than in RootContextData.
            if (context.RootContextData.TryGetValue("__FV_CurrentAccessor", out var a) && a is Lazy<object> accessor)
            {
                propertyContext = new PropertyValidatorContext(context, this, propertyName, accessor);
            }
            else
            {
#pragma warning disable 618
                propertyContext = new PropertyValidatorContext(context, this, propertyName);
#pragma warning restore 618
            }

            // introduced in later version?
            // if (validator.Options.Condition != null && !validator.Options.Condition(propertyContext)) return Enumerable.Empty<ValidationFailure>();

            if (propertyContext.PropertyValue is Optional<TProperty> optional)
            {
                if (!optional.HasValue)
                    return Enumerable.Empty<ValidationFailure>();

                var actualContext = ValidationContext<T>.GetFromNonGenericContext(context);
                var newContext = actualContext.CloneForChildCollectionValidator(context.InstanceToValidate, preserveParentContext: true);
                newContext.PropertyChain.Add(propertyName);

                var newPropertyContext = new PropertyValidatorContext(newContext, this, newContext.PropertyChain.ToString(), optional.Value);
                return validator.Validate(newPropertyContext);
            }
            else
                return Enumerable.Empty<ValidationFailure>();
        }

        protected override async Task<IEnumerable<ValidationFailure>> InvokePropertyValidatorAsync(IValidationContext context, IPropertyValidator validator, string propertyName, CancellationToken cancellation)
        {
            if (string.IsNullOrEmpty(propertyName))
                propertyName = InferPropertyName(Expression);

            PropertyValidatorContext propertyContext;
            // TODO: For FV10 this will come as a parameter rather than in RootContextData.
            if (context.RootContextData.TryGetValue("__FV_CurrentAccessor", out var a) && a is Lazy<object> accessor)
            {
                propertyContext = new PropertyValidatorContext(context, this, propertyName, accessor);
            }
            else
            {
#pragma warning disable 618
                propertyContext = new PropertyValidatorContext(context, this, propertyName);
#pragma warning restore 618
            }

            // introduced in later version?
            // if (validator.Options.Condition != null && !validator.Options.Condition(propertyContext)) return Enumerable.Empty<ValidationFailure>();
            // if (validator.Options.AsyncCondition != null && !await validator.Options.AsyncCondition(propertyContext, cancellation)) return Enumerable.Empty<ValidationFailure>();

            if (propertyContext.PropertyValue is Optional<TProperty> optional)
            {
                if (!optional.HasValue)
                    return Enumerable.Empty<ValidationFailure>();

                var actualContext = ValidationContext<T>.GetFromNonGenericContext(context);
                var newContext = actualContext.CloneForChildCollectionValidator(context.InstanceToValidate, preserveParentContext: true);
                newContext.PropertyChain.Add(propertyName);

                var newPropertyContext = new PropertyValidatorContext(newContext, this, newContext.PropertyChain.ToString(), optional.Value);
                return await validator.ValidateAsync(newPropertyContext, cancellation);
            }
            else
                return Enumerable.Empty<ValidationFailure>();
        }

        private string InferPropertyName(LambdaExpression expression)
        {
            var paramExp = expression.Body as ParameterExpression;

            if (paramExp == null)
                throw new InvalidOperationException("Could not infer property name for expression: " + expression + ". Please explicitly specify a property name by calling OverridePropertyName as part of the rule chain. Eg: RuleForOptional(x => x).NotNull().OverridePropertyName(\"MyProperty\")");

            return paramExp.Name;
        }
    }
}
