namespace RealEstate.Application.Exceptions;

public sealed class ValidationAppException : AppException
{
    public ValidationAppException(string message)
        : base(message)
    {
    }
}
