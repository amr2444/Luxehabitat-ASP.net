namespace RealEstate.Application.Exceptions;

public sealed class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string message)
        : base(message)
    {
    }
}
