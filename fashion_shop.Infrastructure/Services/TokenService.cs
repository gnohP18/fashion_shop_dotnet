using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Core.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace fashion_shop.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenSettings _tokenSettings;
        private readonly UserManager<User> _userManager;

        public TokenService(
            IOptions<TokenSettings> tokenSettings,
            UserManager<User> userManager)
        {
            _tokenSettings = tokenSettings.Value ?? throw new ArgumentNullException(nameof(tokenSettings.Value));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<string> GenerateToken(User user, string jti)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Key));

            var signinCredentials = new SigningCredentials(secretKey, _tokenSettings.SigninCredentials);

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("username", user.UserName ?? ""),
                new Claim(ClaimTypes.Role, string.Join("|", roles)),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_tokenSettings.AccessTokenExpirationHours),
                // Expires = DateTime.UtcNow.AddSeconds(10),
                SigningCredentials = signinCredentials,
                Issuer = _tokenSettings.Issuer,
                Audience = _tokenSettings.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(User user, string jti)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Key));

            var signinCredentials = new SigningCredentials(secretKey, _tokenSettings.SigninCredentials);

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_tokenSettings.AccessTokenExpirationHours + AuthConstant.BONUS_HOUR_REFRESH_TOKEN),
                // Expires = DateTime.UtcNow.AddSeconds(120),
                SigningCredentials = signinCredentials,
                Issuer = _tokenSettings.Issuer,
                Audience = _tokenSettings.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var refreshToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(refreshToken);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_tokenSettings.Key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnAuthorizedException("Token is expired");
            }
            catch (SecurityTokenValidationException ex)
            {
                throw new UnAuthorizedException($"Authentication Failed : {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new UnAuthorizedException($"Authentication Failed : {ex.Message}");
            }
        }
    }
}