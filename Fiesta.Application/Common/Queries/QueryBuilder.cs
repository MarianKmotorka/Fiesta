using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Fiesta.Application.Common.Queries
{
    public static class QueryBuilder
    {
        public static async Task<QueryResponse<T>> BuildResponse<T>(this IQueryable<T> dbQuery, QueryDocument document, CancellationToken cancellationToken)
        {
            var filtered = dbQuery.ApplyFilters(document);

            var totalEntries = await filtered.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalEntries / document.PageSize);

            var entries = await filtered.ApplySorts(document).ApplyPagination(document).ToListAsync(cancellationToken);

            return new QueryResponse<T>(entries)
            {
                Page = document.Page,
                PageSize = document.PageSize,
                TotalEntries = totalEntries,
                TotalPages = totalPages
            };
        }

        public static IQueryable<T> BuildQuery<T>(this IQueryable<T> dbQuery, QueryDocument document)
        {
            return dbQuery.ApplyFilters(document).ApplySorts(document).ApplyPagination(document);
        }

        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> dbQuery, QueryDocument document)
        {
            if (document.Filters != null)
            {
                foreach (var filter in document.Filters)
                    dbQuery = dbQuery.ApplyFilter(filter.FieldName, filter.Operation, filter.FieldValue);
            }

            return dbQuery;
        }

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> dbQuery, QueryDocument document)
        {
            return dbQuery.Skip(document.Page * document.PageSize).Take(document.PageSize);
        }

        public static IQueryable<T> ApplySorts<T>(this IQueryable<T> dbQuery, QueryDocument document)
        {
            if (document.Sorts != null)
            {
                for (int i = 0; i < document.Sorts.Count; i++)
                    dbQuery = dbQuery.ApplySort(document.Sorts[i].FieldName, document.Sorts[i].Order, isFirstSort: i == 0);
            }

            return dbQuery;
        }

        private static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string propertyName, SortType order, bool isFirstSort)
        {
            var expression = source.Expression;
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, propertyName);
            var method = order == SortType.Asc ? "OrderBy" : "OrderByDescending";

            if (!isFirstSort)
                method = order == SortType.Asc ? "ThenBy" : "ThenByDescending";

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
            var ignoreCaseExp = Expression.Constant(StringComparison.OrdinalIgnoreCase);

            if (propertyExp.Type == typeof(string))
                containsMethodExp = operation switch
                {
                    Operation.Equals => Expression.Call(propertyExp, StringMethods.Equals, someValue, ignoreCaseExp),
                    Operation.Contains => Expression.Call(propertyExp, StringMethods.Contains, someValue, ignoreCaseExp),
                    Operation.StartsWith => Expression.Call(propertyExp, StringMethods.StartsWith, someValue, ignoreCaseExp),
                    Operation.EndsWith => Expression.Call(propertyExp, StringMethods.EndsWith, someValue, ignoreCaseExp),
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

            if (propertyExp.Type == typeof(bool))
                containsMethodExp = operation switch
                {
                    Operation.Equals => Expression.Equal(propertyExp, someValue),
                    _ => throw new NotSupportedException("Not allowed OperationEnum for bool type"),
                };

            var predicate = Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);
            return query.Where(predicate);
        }
    }
}
