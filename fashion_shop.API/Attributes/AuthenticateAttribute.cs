using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using fashion_shop.Core.Common;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Redis;

namespace fashion_shop.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthenticateAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var service = context.HttpContext.RequestServices.GetService<IAuthenticationFilterService>();

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            try
            {
                await service.CheckAuthentication(context);
            }
            catch (UnAuthorizedException)
            {
                throw;
            }
        }

        public interface IAuthenticationFilterService
        {
            Task CheckAuthentication(AuthorizationFilterContext context);
        }

        internal class AuthenticationFilterService : IAuthenticationFilterService
        {
            private readonly ITokenService _tokenService;
            private readonly IDatabase _redis;

            public AuthenticationFilterService(
                ITokenService tokenService,
                IConnectionMultiplexer connectionMultiplexer)
            {
                _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
                _redis = connectionMultiplexer.GetDatabase();
            }

            public async Task CheckAuthentication(AuthorizationFilterContext context)
            {
                context.HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenValues);
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

                // check token blacklist
                if (await _redis.KeyExistsAsync($"{AuthConstant.ACCESS_TOKEN_BLACK_LIST}_{jti}_{userId}"))
                {
                    throw new UnAuthorizedException("Token Revoked");
                }
                // Remove header
                RemoveHeader(context.HttpContext.Request);

                // Add header 
                AddHeader(context.HttpContext.Request, claim);
            }

            private void RemoveHeader(HttpRequest request)
            {
                if (request.Headers.Any(x => x.Key == "Authorization-UserId"))
                {
                    request.Headers.Remove("Authorization-UserId");
                }

                if (request.Headers.Any(x => x.Key == "Authorization-Username"))
                {
                    request.Headers.Remove("Authorization-Username");
                }
            }

            private void AddHeader(HttpRequest request, ClaimsPrincipal claim)
            {
                var userId = claim.FindFirstValue("userId");
                var username = claim.FindFirstValue("username");

                var jti = claim.FindFirstValue(JwtRegisteredClaimNames.Jti);

                request.Headers.TryAdd("Authorization-UserId", userId);
                request.Headers.TryAdd("Authorization-Username", username);
                request.Headers.TryAdd("Authorization-Jti", jti); ;
            }
        }
    }
}