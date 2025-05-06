using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.API.ExternalService.Entities;
using fashion_shop.Core.Common;

namespace fashion_shop.Core.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public OrderStatusEnum OrderStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }

    public virtual HashSet<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    public virtual HashSet<PaymentTransaction> PaymentTransactions { get; set; } = new HashSet<PaymentTransaction>();
}