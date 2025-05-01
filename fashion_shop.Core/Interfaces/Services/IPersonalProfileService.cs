using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;

namespace fashion_shop.Core.Interfaces.Services;

public interface IPersonalProfileService
{
    Task<GetMeResponse> GetMeAsync(User user);
    Task UpdatePasswordAsync(User user, UpdateProfilePasswordRequest request);
}