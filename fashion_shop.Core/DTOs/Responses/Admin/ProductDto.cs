using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public int Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsVariant { get; set; }
    public List<ProductVariantDto> ProductVariants { get; set; } = new List<ProductVariantDto>();
    public List<ProductItemDto> ProductItems { get; set; } = new List<ProductItemDto>();
}