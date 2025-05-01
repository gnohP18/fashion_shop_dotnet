using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs;
using fashion_shop.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using fashion_shop.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using fashion_shop.API.Controllers.Webhook;

namespace fashion_shop.API.Controllers
{
    [ApiController]
    public abstract class APIController<T> : ControllerBase where T : class
    {
        protected object _data = new object();
        protected readonly ILogger _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly UserManager<User> _userManager;


        protected APIController(ILogger<T> logger, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        protected async Task<User> GetUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new UnAuthorizedException("HttpContext is null");
            }

            if (!httpContext.Request.Headers.TryGetValue("Authorization-UserId", out var userId) ||
                StringValues.IsNullOrEmpty(userId))
            {
                throw new UnAuthorizedException("Not found userId in Header");
            }

            var userIdStr = userId.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(userIdStr))
            {
                throw new UnAuthorizedException("userId in Header is empty");
            }

            var user = await _userManager.FindByIdAsync(userIdStr);

            if (user == null)
            {
                throw new UnAuthorizedException("User not found");
            }

            return user;
        }

        protected string GetJti()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new UnAuthorizedException("HttpContext is null");
            }

            if (!httpContext.Request.Headers.TryGetValue("Authorization-Jti", out var jti) ||
                StringValues.IsNullOrEmpty(jti))
            {
                throw new UnAuthorizedException("Not found userId in Header");
            }

            return jti.ToString();
        }

        protected ActionResult OkResponse<A>(A data, string message) where A : class
        {
            return Ok(new
            {
                status = HttpStatusCode.OK,
                message,
                data
            });
        }

        protected ActionResult CreatedResponse<A>(string message) where A : class
        {
            return Ok(new
            {
                status = HttpStatusCode.Created,
                message,
            });
        }

        protected ActionResult AcceptedResponse<A>(string message) where A : class
        {
            return Ok(new
            {
                status = HttpStatusCode.Accepted,
                message,
            });
        }

        protected ActionResult NoContentResponse<A>(string message) where A : class
        {
            return Ok(new
            {
                status = HttpStatusCode.NoContent,
                message,
            });
        }

        protected ActionResult ErrorResponse<A>(string message) where A : class
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new
            {
                status = HttpStatusCode.BadRequest,
                message,
                data = errors
            });
        }
    }
}