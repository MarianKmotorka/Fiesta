using System;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Fiesta.Application.Common.Queries
{
    public static class QueryBuilder
    {
        public static IQueryable<T> BuildQuery<T>(this IQueryable<T> dbQuery, QueryDocument document)
        {
            if (document.Filters != null)
            {
                foreach (var filter in document.Filters)
                    dbQuery = dbQuery.ApplyFilter(filter.FieldName, filter.Operation, filter.FieldValue);
            }

            if (document.Sorts != null)
            {
                for (int i = 0; i < document.Sorts.Count; i++)
                    dbQuery = dbQuery.OrderBy(document.Sorts[i].FieldName, document.Sorts[i].Order, isFirstSort: i == 0);
            }

            return dbQuery.Skip(document.Page * document.PageSize).Take(document.PageSize);
        }

        private static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, OrderType order, bool isFirstSort)
        {
            var expression = source.Expression;
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, propertyName);
            var method = order == OrderType.Asc ? "OrderBy" : "OrderByDescending";

            if (!isFirstSort)
                method = order == OrderType.Asc ? "ThenBy" : "ThenByDescending";

            expression = Expression.Call(typeof(Queryable), method,
                new Type[] { source.ElementType, selector.Type },
                expression, Expression.Quote(Expression.Lambda(selector, parameter)));
            return source.Provider.CreateQuery<T>(expression);
        }

        private static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, string propertyName, Operation operation, object propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T));
            var propertyExp = Expression.Property(parameterExp, propertyName);

            Expression containsMethodExp = null;

            if (propertyValue is JArray filterValues)
            {
                foreach (var item in filterValues)
                {
                    var expressionValue = Expression.Constant(item.ToObject(propertyExp.Type), propertyExp.Type);
                    var equalMethodExp = Expression.Equal(propertyExp, expressionValue);

                    if (containsMethodExp == null)
                        containsMethodExp = equalMethodExp;
                    else
                        containsMethodExp = Expression.Or(containsMethodExp, equalMethodExp);
                }

                return query.Where(Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp));
            };

            if (propertyExp.Type == typeof(Guid))
            {
                if (!Guid.TryParse((string)propertyValue, out var guidValue))
                    return query;

                var expressionConstant = Expression.Constant(guidValue, propertyExp.Type);
                containsMethodExp = Expression.Equal(propertyExp, expressionConstant);

                return query.Where(Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp));
            }

            if (propertyExp.Type.IsEnum)
            {
                var serializedValue = JToken.FromObject(propertyValue);
                var convertedExpr = Expression.Constant(Convert.ChangeType(serializedValue, propertyExp.Type), propertyExp.Type);
                containsMethodExp = operation switch
                {
                    Operation.Equals => Expression.Equal(propertyExp, convertedExpr),
                    Operation.HasFlag => Expression.Call(propertyExp, EnumMethods.HasFlag, Expression.Convert(convertedExpr, typeof(Enum))),
                    _ => throw new NotSupportedException("Not allowed operation for enum type"),
                };

                return query.Where(Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp));
            }

            var someValue = Expression.Constant(Convert.ChangeType(propertyValue, propertyExp.Type), propertyExp.Type);

            if (propertyExp.Type == typeof(string))
                containsMethodExp = operation switch
                {
                    Operation.Equals => Expression.Equal(propertyExp, someValue),
                    Operation.Contains => Expression.Call(propertyExp, StringMethods.Contains, someValue),
                    Operation.StartsWith => Expression.Call(propertyExp, StringMethods.StartsWith, someValue),
                    Operation.EndsWith => Expression.Call(propertyExp, StringMethods.EndsWith, someValue),
                    _ => throw new NotSupportedException("Not allowed OperationEnum for string type"),
                };

            if (propertyExp.Type == typeof(int) || propertyExp.Type == typeof(uint))
                containsMethodExp = operation switch
                {
                    Operation.Equals => Expression.Equal(propertyExp, someValue),
                    Operation.GreaterThan => Expression.GreaterThan(propertyExp, someValue),
                    Operation.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyExp, someValue),
                    Operation.LessThan => Expression.LessThan(propertyExp, someValue),
                    Operation.LessThanOrEqual => Expression.LessThanOrEqual(propertyExp, someValue),
                    _ => throw new NotSupportedException("Not allowed OperationEnum for number type"),
                };

            if (propertyExp.Type == typeof(DateTime))
                containsMethodExp = operation switch
                {
                    Operation.Equals => Expression.Equal(propertyExp, someValue),
                    Operation.GreaterThan => Expression.GreaterThan(propertyExp, someValue),
                    Operation.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyExp, someValue),
                    Operation.LessThan => Expression.LessThan(propertyExp, someValue),
                    Operation.LessThanOrEqual => Expression.LessThanOrEqual(propertyExp, someValue),
                    _ => throw new NotSupportedException("Not allowed OperationEnum for datetime type"),
                };

            var predicate = Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);

            return query.Where(predicate);
        }
    }
}
