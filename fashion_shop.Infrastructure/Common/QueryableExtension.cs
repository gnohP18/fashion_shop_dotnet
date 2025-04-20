using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;

namespace fashion_shop.Infrastructure.Common
{
    public static class QueryableExtension
    {
        public static IQueryable<T> WithoutDeleted<T>(this IQueryable<T> source) where T : BaseEntity
        {
            return source.Where(e => !e.IsDeleted);
        }

        public static IQueryable<T> WithDeleted<T>(this IQueryable<T> source) where T : BaseEntity
        {
            return source.Where(e => e.IsDeleted);
        }
    }
}