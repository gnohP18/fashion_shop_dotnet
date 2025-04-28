using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class UpdateStatisticSettingRequest
{
    public int TopSeller { get; set; }
    public int RecentOrder { get; set; }
    public StatisticEnum SaleRevenue { get; set; }
}