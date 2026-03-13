namespace RealEstate.Application.Exceptions;

public sealed class ConflictAppException : AppException
{
    public ConflictAppException(string message)
        : base(message)
    {
    }
}
