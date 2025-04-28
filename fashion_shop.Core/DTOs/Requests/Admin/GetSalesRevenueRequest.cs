using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class GetSalesRevenueRequest
{
    public StatisticEnum Mode { get; set; } = StatisticEnum.Week;
}