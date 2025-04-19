using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses.Admin;

public class ProductVariantDto
{
    public string Name { get; set; } = default!;
    public int Priority { get; set; }
    public List<VariantDto> Variants { get; set; } = new List<VariantDto>();
}