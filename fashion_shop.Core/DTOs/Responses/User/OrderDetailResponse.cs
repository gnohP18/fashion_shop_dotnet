using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses.User;

public class OrderDetailResponse
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public string? CreatedAt { get; set; }
    public List<OrderDetailDto> OrderDetail { get; set; } = new List<OrderDetailDto>();
}