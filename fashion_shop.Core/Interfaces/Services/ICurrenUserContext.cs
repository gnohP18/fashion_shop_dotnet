using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Interfaces.Services;

public interface ICurrenUserContext
{
    public string? UserId { get; }
    public string? Username { get; }
    public string? Jti { get; }
    public string? FcmToken { get; }
}