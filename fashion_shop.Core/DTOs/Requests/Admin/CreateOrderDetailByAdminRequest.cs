using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class CreateOrderDetailByAdminRequest
{
    public int ProductItemId { get; set; }
    public int Quantity { get; set; }
}