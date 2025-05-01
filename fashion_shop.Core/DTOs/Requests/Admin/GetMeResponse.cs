using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Responses.User;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class GetMeResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public List<string> Role { get; set; } = [];
    public string PhoneNumber { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public StatisticSettingResponse? StatisticSetting { get; set; }
}