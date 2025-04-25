using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.DTOs.Responses.User;

public class OrderDetailDto
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int ProductItemId { get; set; }
    public string ProductItemCode { get; set; } = default!;
    public string ProductName { get; set; } = default!;
    public string ProductSlug { get; set; } = default!;
    public int Quantity { get; set; }
    public int Price { get; set; }
    public string? CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
    public List<VariantObject> VariantObjects { get; set; } = new List<VariantObject>();
}