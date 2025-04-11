using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}