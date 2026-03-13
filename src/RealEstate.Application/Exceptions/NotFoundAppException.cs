namespace RealEstate.Application.Exceptions;

public sealed class NotFoundAppException : AppException
{
    public NotFoundAppException(string message)
        : base(message)
    {
    }
}
