using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class UpdateProductItemRequest
{
    public int Id { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
}

