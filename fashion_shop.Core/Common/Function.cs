using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Common
{
    public static class Function
    {
        /// <summary>
        /// Generate Slug depend on current time
        /// </summary>
        /// <param name="slug">Producr Slug</param>
        /// <returns>Generated Slug</returns>
        public static string GenerateSlugProduct(string slug)
        {
            return $"{slug}-{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}";
        }

        /// <summary>
        /// Format VietNam Dong
        /// </summary>
        /// <param name="amount">Money Amount</param>
        /// <returns>Format Money</returns>
        public static string FormatVnd(decimal amount)
        {
            return string.Format("{0:N0}₫", amount);
        }
    }
}