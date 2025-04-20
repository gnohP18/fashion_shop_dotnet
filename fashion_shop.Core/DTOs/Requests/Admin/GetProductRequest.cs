using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin
{
    public class GetProductRequest : QueryRequest
    {
        public string? KeySearch { get; set; }
        public string? CategorySlug { get; set; }
    }
}