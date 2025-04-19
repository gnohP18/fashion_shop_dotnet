using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public int Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
    public bool IsVariant { get; set; }

    public virtual HashSet<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    public virtual HashSet<ProductVariant> ProductVariants { get; set; } = new HashSet<ProductVariant>();
    public virtual HashSet<ProductItem> ProductItems { get; set; } = new HashSet<ProductItem>();
}