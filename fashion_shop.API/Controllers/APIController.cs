using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers
{
    [ApiController]
    public abstract class APIController<T> : ControllerBase where T : class
    {
        protected object _data = new object();
        protected readonly ILogger _logger;

        protected APIController(ILogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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