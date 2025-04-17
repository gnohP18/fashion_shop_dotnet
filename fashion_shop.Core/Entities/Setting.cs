using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class Setting : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? DefaultValue { get; set; }
}