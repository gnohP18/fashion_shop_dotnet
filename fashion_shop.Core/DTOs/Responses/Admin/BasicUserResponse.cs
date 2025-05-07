using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Responses.Admin;

public class BasicUserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string RoleName { get; set; } = default!;
    public string? ImageUrl { get; set; }
}