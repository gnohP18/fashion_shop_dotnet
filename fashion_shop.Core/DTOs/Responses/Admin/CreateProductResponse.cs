using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.DTOs.Responses.Admin;

public class CreateProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Price { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public CategoryDto Category { get; set; } = default!;
}