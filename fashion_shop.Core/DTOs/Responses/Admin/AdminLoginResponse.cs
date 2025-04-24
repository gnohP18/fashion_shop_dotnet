using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses.Admin;

public class AdminLoginResponse
{
    public string access_token { get; set; } = default!;
    public string refresh_token { get; set; } = default!;
    public int expires_in { get; set; }
}