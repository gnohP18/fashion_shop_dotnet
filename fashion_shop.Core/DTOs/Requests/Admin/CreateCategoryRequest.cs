using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
    }
}