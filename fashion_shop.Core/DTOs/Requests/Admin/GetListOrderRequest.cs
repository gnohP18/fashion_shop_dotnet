using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class GetListOrderRequest : QueryRequest
{
    public string? KeySearch { get; set; }
    public int? MaxPrice { get; set; }
    public int? MinPrice { get; set; }
    public DateTime? MaxDate { get; set; }
    public DateTime? MinDate { get; set; }
}