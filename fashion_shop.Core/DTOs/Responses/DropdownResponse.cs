using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses;

public class DropdownResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}