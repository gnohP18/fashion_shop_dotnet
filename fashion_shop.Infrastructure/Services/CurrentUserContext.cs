using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace fashion_shop.Infrastructure.Services;

public class CurrentUserContext : ICurrenUserContext
{
    public string? UserId { get; }

    public string? Username { get; }

    public string? Jti { get; }

    public string? FcmToken { get; }


    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {

        var headers = httpContextAccessor.HttpContext?.Request?.Headers;

        if (headers != null)
        {
            headers.TryGetValue("Authorization-UserId", out var userId);
            headers.TryGetValue("Authorization-Username", out var username);
            headers.TryGetValue("Authorization-Username", out var jti);
            headers.TryGetValue("fcm_token", out var fcmToken);

            UserId = userId.ToString();
            Username = username.ToString();
            FcmToken = fcmToken.ToString();
            Jti = jti.ToString();
        }
    }
}