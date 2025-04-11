using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Common
{
    public static class PaginationConstant
    {
        public const int PageSize = 15;
        public const int PageStart = 1;
        public const string DefaultSortKey = "Id";
        public const string DefaultSortDirection = "DESC";
    }
}