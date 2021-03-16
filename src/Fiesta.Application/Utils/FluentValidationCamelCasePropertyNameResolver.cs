using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;

namespace Fiesta.Application.Utils
{
    public static class FluentValidationCamelCasePropertyNameResolver
    {
        public static void UseFluentValidationCamelCasePropertyResolver()
        {
            ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver;
        }

        private static string CamelCasePropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression)
        {
            if (expression != null)
            {
                var chain = FromExpression(expression);

                return chain.Count switch
                {
                    0 => "",
                    1 => ToCamelCase(chain.First()),
                    _ => string.Join(ValidatorOptions.Global.PropertyChainSeparator, chain.Select(ToCamelCase))
                };
            }

            return ToCamelCase(memberInfo?.Name);
        }

        private static IReadOnlyCollection<string> FromExpression(LambdaExpression expression)
        {
            var memberNames = new Stack<string>();

            var getMemberExp = new Func<Expression, MemberExpression>(toUnwrap =>
            {
                if (toUnwrap is UnaryExpression)
                {
                    return ((UnaryExpression)toUnwrap).Operand as MemberExpression;
                }

                return toUnwrap as MemberExpression;
            });

            var memberExp = getMemberExp(expression.Body);

            while (memberExp != null)
            {
                memberNames.Push(memberExp.Member.Name);
                memberExp = getMemberExp(memberExp.Expression);
            }

            return memberNames;
        }

        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value) || !char.IsUpper(value[0]))
            {
                return value;
            }

            var chars = value.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                    break;

                bool hasNext = i + 1 < chars.Length;
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;

                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }

            return new string(chars);
        }
    }
}
