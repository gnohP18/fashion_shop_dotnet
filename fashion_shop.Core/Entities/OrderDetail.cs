using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class OrderDetail : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int Price { get; set; }
}