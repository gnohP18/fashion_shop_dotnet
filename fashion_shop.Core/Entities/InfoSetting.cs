using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class InfoSetting
{
    public string ShopName { get; set; } = default!;
    public string ShopEmail { get; set; } = default!;
    public string ShopPhone { get; set; } = default!;
    public string ShopAddress { get; set; } = default!;
}