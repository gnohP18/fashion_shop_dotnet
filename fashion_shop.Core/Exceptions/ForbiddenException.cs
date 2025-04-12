namespace fashion_shop.Core.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base() { }
    public ForbiddenException(string message) : base(message) { }
}