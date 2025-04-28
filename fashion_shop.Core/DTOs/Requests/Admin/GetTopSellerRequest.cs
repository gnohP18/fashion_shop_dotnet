using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class GetTopSellerRequest
{
    public int numberOfProduct { get; set; } = 3;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}