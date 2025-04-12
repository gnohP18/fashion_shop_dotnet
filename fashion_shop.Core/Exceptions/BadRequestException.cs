using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException() : base() { }
    public BadRequestException(string message) : base(message) { }
}