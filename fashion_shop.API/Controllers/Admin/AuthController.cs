using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using fashion_shop.API.Attributes;
using fashion_shop.Core.DTOs;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.fashion_shop.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers.Admin
{
    [ApiController]
    [Tags("Authentication")]
    [Route("api/admin/auth")]
    public class AuthController : APIController<AuthController>
    {
        private readonly IAdminAuthService _adminAuthService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(
            IAdminAuthService adminAuthService,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService,
            ILogger<AuthController> logger) : base(logger)
        {
            _adminAuthService = adminAuthService ?? throw new ArgumentNullException(nameof(adminAuthService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost("login")]
        public async Task<BaseResponse<AdminLoginResponse>> Login([FromBody] AdminLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return HandleInvalidModel<AdminLoginResponse>();
            }

            var data = await _adminAuthService.LoginAsync(request);

            return new BaseResponse<AdminLoginResponse>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Login successfully",
                Data = data,
            };
        }

        [HttpPost("logout")]
        // [Authenticate]
        public async Task<BaseResponse<string>> Logout()
        {
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenValues);
            var token = tokenValues.FirstOrDefault();

            if (tokenValues.Count == 0 || string.IsNullOrWhiteSpace(token))
            {
                throw new UnAuthorizedException("Missing Authenticate key");
            }

            var claim = _tokenService.ValidateToken(token);

            if (claim is null)
            {
                throw new UnAuthorizedException("UnAuthorization");
            }

            var userId = claim.FindFirstValue("userId");

            var jti = claim.FindFirstValue(JwtRegisteredClaimNames.Jti);

            return await _adminAuthService.LogoutAsync(userId, jti);
        }

        [HttpPost("refresh")]
        public async Task<BaseResponse<AdminLoginResponse>> Refresh([FromBody] AdminRefreshLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return HandleInvalidModel<AdminLoginResponse>();
            }

            var data = await _adminAuthService.RefreshLoginAsync(request);

            return new BaseResponse<AdminLoginResponse>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Refresh login successfully",
                Data = data,
            };
        }
    }
}