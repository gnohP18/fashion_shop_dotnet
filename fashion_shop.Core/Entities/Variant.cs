using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class Variant : BaseEntity
{
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = default!;

    public string Code { get; set; } = default!;
    public string Value { get; set; } = default!;
}