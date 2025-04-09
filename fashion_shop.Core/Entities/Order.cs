using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int TotalAmount { get; set; }
    public string Note { get; set; } = string.Empty;

    public virtual HashSet<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
}