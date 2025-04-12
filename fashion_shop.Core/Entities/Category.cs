using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;

    public virtual HashSet<Product> Products { get; set; } = new HashSet<Product>();
}