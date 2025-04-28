using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class CreateOrderByAdminRequest
{
    [Required(ErrorMessage = "Phone is required")]
    [MinLength(10, ErrorMessage = "Phone must be at least 10 characters")]
    public string Phone { get; set; } = default!;
    public bool IsGuest { get; set; } = true;
    public List<CreateOrderDetailByAdminRequest> OrderDetails { get; set; } = new List<CreateOrderDetailByAdminRequest>();
    public string? Note { get; set; }
}