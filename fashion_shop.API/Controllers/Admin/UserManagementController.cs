using fashion_shop.API.Attributes;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers.Admin
{
    [ApiController]
    [Tags("User Management")]
    [Route("api/user-management")]
    public class UserManagementController : APIController<UserManagementController>
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(
            ILogger<UserManagementController> logger,
            IUserManagementService userManagementService,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager) : base(logger, httpContextAccessor, userManager)
        {
            _userManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
        }

        [HttpGet("check-user-phone")]
        public async Task<bool> CheckExistUserPhone(string phone)
        {
            return await _userManagementService.CheckExistUserPhoneAsync(phone);
        }

        [HttpGet("check-user-username")]
        public async Task<bool> CheckExistUserUsername(string username)
        {
            return await _userManagementService.CheckExistUserUsernameAsync(username);
        }

        [HttpGet("get-user-order")]
        public async Task<IActionResult> GetUserForOrder(string phone)
        {
            var data = await _userManagementService.GetUserForOrderByPhoneAsync(phone);

            if (data is null)
            {
                throw new NotFoundException($"Not found User with phone ={phone}");
            }

            return OkResponse<UserForOrderResponse>(data, "Get data successfully");
        }

        [HttpGet("")]
        [Authenticate]
        public async Task<IActionResult> GetAsync([FromQuery] GetListUserRequest request)
        {
            return OkResponse<PaginationData<BasicUserResponse>>(await _userManagementService.GetListUserAsync(request), "Get List User successfully");
        }
    }
}