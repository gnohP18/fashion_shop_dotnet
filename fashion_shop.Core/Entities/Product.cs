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
    public string ImageUrl { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;

    public int CategoryId { get; set; } = default!;
    public Category Category { get; set; } = default!;

    public virtual HashSet<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
}