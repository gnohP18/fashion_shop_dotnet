using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;

namespace fashion_shop.Core.DTOs.Requests;

public class QueryRequest
{
    public int Limit { get; set; } = PaginationConstant.PageSize;
    public int Page { get; set; } = PaginationConstant.PageStart;
    public string SortBy { get; set; } = PaginationConstant.DefaultSortKey;
    public string Direction { get; set; } = PaginationConstant.DefaultSortDirection;
}