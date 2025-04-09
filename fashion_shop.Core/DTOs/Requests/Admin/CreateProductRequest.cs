using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = String.Empty;
        public string Slug { get; set; } = String.Empty;
        public string Price { get; set; } = String.Empty;
        public string ImageUrl { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public int CategoryId { get; set; }
    }
}