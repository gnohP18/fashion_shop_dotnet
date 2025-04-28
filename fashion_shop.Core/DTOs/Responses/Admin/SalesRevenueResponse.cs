using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses.Admin;

public class SalesRevenueResponse
{
    public string Label { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public int TotalItem { get; set; }
}