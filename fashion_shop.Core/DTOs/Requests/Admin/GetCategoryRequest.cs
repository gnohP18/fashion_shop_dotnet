using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin
{
    public class GetCategoryRequest : QueryRequest
    {
        public string? KeySearch { get; set; }
    }
}