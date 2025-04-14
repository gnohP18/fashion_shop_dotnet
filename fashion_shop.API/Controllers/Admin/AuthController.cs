using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using fashion_shop.Core.DTOs;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
        public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<AdminLoginResponse>("Validation Failed");
            }

            var data = await _adminAuthService.LoginAsync(request);

            return SuccessResponse<AdminLoginResponse>(data, "Login successfully");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var tokenValues = default(StringValues);

            _httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out tokenValues);

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

            var userId = claim.FindFirstValue("userId") ?? throw new UnAuthorizedException("Invalid Tokenn : Missing userId");

            var jti = claim.FindFirstValue(JwtRegisteredClaimNames.Jti) ?? throw new UnAuthorizedException("Invalid Tokenn: Missing Jti");

            await _adminAuthService.LogoutAsync(userId, jti);

            return SuccessResponse<string>(string.Empty, "Logout successfully");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] AdminRefreshLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<AdminLoginResponse>("Validation Failed");
            }

            var data = await _adminAuthService.RefreshLoginAsync(request);

            return SuccessResponse<AdminLoginResponse>(data, "Refresh login successfully");
        }
    }
}