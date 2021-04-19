using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Reflection.BindingFlags;

namespace TestBase.Helpers
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Can be used to set value of non-public property or field.
        /// </summary>
        /// <remarks>
        /// Does not provide compile time check. Can break when member is removed or renamed, or its type has changed.
        /// </remarks>
        public static TEntity Set<TEntity>(this TEntity entity, string name, object value)
        {
            var member = typeof(TEntity)
                .GetMember(name, GetField | GetProperty | NonPublic | Public | Instance | IgnoreCase)
                .FirstOrDefault();

            switch (member)
            {
                case PropertyInfo property when property.CanWrite:
                    property.SetValue(entity, value, null);
                    return entity;

                case FieldInfo field:
                    field.SetValue(entity, value);
                    return entity;
            }

            throw new NotSupportedException($"Member {name} must be property with setter or field.");
        }

        /// <summary>
        /// Can be used to set value of property with non-public setter.
        /// </summary>
        public static TEntity Set<TEntity, TProperty>(this TEntity entity, Expression<Func<TEntity, TProperty>> expression, TProperty value)
        {
            if (expression.Body is MemberExpression memberExpression
                && memberExpression.Member is PropertyInfo property
                && property.CanWrite)
            {
                property.SetValue(entity, value, null);
                return entity;
            }

            throw new NotSupportedException($"Member expression {expression} must be property with setter.");
        }
    }
}
