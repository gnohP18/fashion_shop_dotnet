using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses.User;

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Code { get; set; } = default!;
    public string ProductName { get; set; } = default!;
    public string ProductSlug { get; set; } = default!;
    public int Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public bool IsVariant { get; set; }
    public List<string> Variants { get; set; } = new List<string>();
}