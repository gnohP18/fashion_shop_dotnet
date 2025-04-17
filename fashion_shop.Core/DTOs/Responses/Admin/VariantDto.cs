using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses.Admin;

public class VariantDto
{
    public string Code { get; set; } = default!;
    public string Value { get; set; } = default!;
}