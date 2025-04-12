using fashion_shop.Core.DTOs;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;

namespace fashion_shop.Core.Interfaces.Services;

public interface IAdminAuthService
{
    Task<AdminLoginResponse> LoginAsync(AdminLoginRequest request);
    Task<BaseResponse<string>> LogoutAsync(string userId, string jti);
    Task<AdminLoginResponse> RefreshLoginAsync(AdminRefreshLoginRequest request);
}