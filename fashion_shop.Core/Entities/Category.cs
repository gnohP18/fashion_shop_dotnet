using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = String.Empty;
    public string Slug { get; set; } = String.Empty;

    public virtual HashSet<Product> Products{ get; set; } = new HashSet<Product>();
}