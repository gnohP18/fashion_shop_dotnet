using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public string Name { get; set; } = default!;
    public int Priority { get; set; }

    public HashSet<Variant> Variants { get; set; } = new HashSet<Variant>();
}