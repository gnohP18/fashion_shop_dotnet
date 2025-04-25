using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.DTOs.Responses.Admin;

public class ProductItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Code { get; set; } = default!;
    public int Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public List<VariantObject> VariantObjects { get; set; } = new List<VariantObject>();
}