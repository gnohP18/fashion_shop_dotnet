using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Exceptions;

public class UnprocessableContentException : Exception
{
    public UnprocessableContentException() : base() { }
    public UnprocessableContentException(string message) : base(message) { }
}