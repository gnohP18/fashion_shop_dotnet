using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses.Admin;

public class ProductItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Code { get; set; } = default!;
    public int Price { get; set; }
    public string? ImageUrl { get; set; }
}