using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class ProductItem : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public string Code { get; set; } = default!;
    public int Price { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }

    public virtual HashSet<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
}