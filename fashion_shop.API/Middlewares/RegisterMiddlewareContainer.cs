using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.API.Middlewares;

namespace SampleDotNet.Api.Middlewares
{
    public static class RegisterMiddlewareContainer
    {
        public static IApplicationBuilder UseRegisterMiddleware(this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}