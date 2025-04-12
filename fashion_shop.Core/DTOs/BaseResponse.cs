using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs;

public class BaseResponse<T> where T : class
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.NoContent;
    public string Message { get; set; } = null!;
    public T Data { get; set; } = null!;

    public object? Errors { get; set; }

    public object? Meta { get; set; }
}

public class BaseResponse : BaseResponse<object>
{
}