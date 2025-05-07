using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;

namespace fashion_shop.Core.Interfaces.Services;

public interface IUserManagementService
{
    Task<bool> CheckExistUserPhoneAsync(string phone);
    Task<bool> CheckExistUserUsernameAsync(string username);
    Task<UserForOrderResponse?> GetUserForOrderByPhoneAsync(string phone);
    Task<PaginationData<BasicUserResponse>> GetListUserAsync(GetListUserRequest request);
}