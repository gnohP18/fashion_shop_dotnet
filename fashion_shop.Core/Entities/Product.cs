using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = String.Empty;
    public string Slug { get; set; } = String.Empty;
    public string Price { get; set; } = String.Empty;
    public string ImageUrl { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public virtual HashSet<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
}