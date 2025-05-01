using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Extensions;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using fashion_shop.Core.DTOs.Common;
using Microsoft.Extensions.Options;

namespace fashion_shop.Infrastructure.Services
{
    public class AdminAuthService : IAdminAuthService
    {
        private readonly ILogger _logger;
        private readonly IDatabase _redis;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly UserManager<User> _userManager;
        private readonly TokenSettings _tokenSettings;


        public AdminAuthService(
            ILogger<AdminAuthService> logger,
            IConnectionMultiplexer connectionMultiplexer,
            ITokenService tokenService,
            IPasswordHasher<User> passwordHasher,
            UserManager<User> userManager,
            IOptions<TokenSettings> tokenSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _redis = connectionMultiplexer.GetDatabase() ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenSettings = tokenSettings.Value ?? throw new ArgumentNullException(nameof(tokenSettings.Value));
        }

        public async Task<AdminLoginResponse> LoginAsync(AdminLoginRequest request)
        {
            _logger.LogInformation("Start LoginAsync");
            // Check request
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
            {
                throw new NotFoundException("Not found user");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Any(r => r.ToString() == RoleEnum.Admin.GetDisplayName() || r.ToString() == RoleEnum.Manager.GetDisplayName()))
            {
                throw new ForbiddenException("You are not authorized to access the admin panel");
            }

            var resultCheckPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? "", request.Password);

            if (resultCheckPassword == PasswordVerificationResult.Failed)
            {
                throw new UnAuthorizedException("Wrong Password");
            }

            var jti = Guid.NewGuid().ToString();

            var accessToken = await _tokenService.GenerateToken(user, jti);

            var refreshToken = _tokenService.GenerateRefreshToken(user, jti);

            // AddRefreshTokenCookie(refreshToken);

            _logger.LogInformation("End LoginAsync");

            return new AdminLoginResponse
            {
                access_token = accessToken,
                refresh_token = refreshToken,
                expires_in = (_tokenSettings.AccessTokenExpirationHours + AuthConstant.BONUS_HOUR_REFRESH_TOKEN) * 60 * 60
            };
        }

        public async Task<BaseResponse<string>> LogoutAsync(string userId, string jti)
        {
            var key = $"{AuthConstant.ACCESS_TOKEN_BLACK_LIST}_{jti}_{userId}";

            // Revoke token
            await _redis.StringSetAsync(key, 1);

            return new BaseResponse<string>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Token revoked",
            };
        }

        public async Task<AdminLoginResponse> RefreshLoginAsync(AdminRefreshLoginRequest request)
        {
            // Ý tưởng
            // 1. Lấy claim từ refresh token
            // 2. Kiểm tra xem trong redis có tồn tại jti không
            // 3. Nếu có throw exception, thông báo mail, .v.v
            // 4. Nếu không add jti vào trong redis và cấp access token, refresh token mới 

            // 1. Lấy claim từ refresh token
            var claim = _tokenService.ValidateToken(request.RefreshToken);

            if (claim == null)
            {
                throw new UnAuthorizedException("Invalid Refresh Token");
            }

            var userId = claim.FindFirstValue("userId");

            var user = await _userManager.FindByIdAsync(userId ?? "");
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var jti = claim.FindFirstValue(JwtRegisteredClaimNames.Jti);

            // 2. Kiểm tra xem trong redis có tồn tại jti không
            if (await _redis.KeyExistsAsync($"{AuthConstant.REFRESH_TOKEN_BLACK_LIST}_{jti}_{userId}"))
            {
                // 3. Nếu có throw exception, thông báo mail, .v.v
                throw new UnAuthorizedException("Refresh token is expired, please login again");
            }

            // 4. Nếu không add jti vào trong redis và cấp access token, refresh token mới 
            var key = $"{AuthConstant.REFRESH_TOKEN_BLACK_LIST}_{jti}_{userId}";
            // 4.1 Revoke refresh token
            await _redis.StringSetAsync(key, 1);

            var newJti = Guid.NewGuid().ToString();

            var accessToken = await _tokenService.GenerateToken(user, newJti);

            var refreshToken = _tokenService.GenerateRefreshToken(user, newJti);

            return new AdminLoginResponse
            {
                access_token = accessToken,
                refresh_token = refreshToken,
                expires_in = (_tokenSettings.AccessTokenExpirationHours + AuthConstant.BONUS_HOUR_REFRESH_TOKEN) * 60 * 60
            };
        }
    }
}