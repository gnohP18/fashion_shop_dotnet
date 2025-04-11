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

        protected BaseResponse<A> HandleInvalidModel<A>() where A : class
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new BaseResponse<A>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Validation failed",
                Errors = errors
            };
        }
    }
}