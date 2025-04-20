using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class UpdateBasicInfoSettingRequest
{
    public string ShopName { get; set; } = default!;
    public string ShopEmail { get; set; } = default!;
    public string ShopPhone { get; set; } = default!;
    public string ShopAddress { get; set; } = default!;
}