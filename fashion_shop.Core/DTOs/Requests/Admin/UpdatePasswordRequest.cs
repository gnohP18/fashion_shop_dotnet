using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class UpdateProfilePasswordRequest
{
    public string Password { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}