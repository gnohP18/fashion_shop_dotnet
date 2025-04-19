using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace fashion_shop.Infrastructure.Common
{
    public static class Function
    {
        public static int GetMilliSecondFromStartYear()
        {
            var now = DateTime.UtcNow;

            var startOfYear = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (int)(now - startOfYear).TotalMilliseconds;
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string orderByProperty, bool desc = true) where T : class
        {
            // Lấy Type của T
            var type = typeof(T);

            // Không phân biệt hoa thường
            var property = type.GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new ArgumentException($"Property '{orderByProperty}' not found on type '{type.Name}'");

            var parameter = Expression.Parameter(type, "x"); // x

            var propertyAccess = Expression.Property(parameter, property); // x.property 

            var orderByExpression = Expression.Lambda(propertyAccess, parameter); // (x => x.property)

            var methodName = desc ? "OrderByDescending" : "OrderBy";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { type, property.PropertyType },
                source.Expression,
                Expression.Quote(orderByExpression)
            );

            return source.Provider.CreateQuery<T>(resultExpression);
        }
    }
}