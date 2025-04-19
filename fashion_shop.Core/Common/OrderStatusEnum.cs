using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Common;

public enum OrderStatusEnum
{
    New = 0,
    Processing = 10,
    Cancelled = 30,
    Completed = 90,
}