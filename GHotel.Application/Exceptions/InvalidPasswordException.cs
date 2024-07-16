namespace GHotel.Application.Exceptions;

public class InvalidPasswordException : OperationFailedException
{
    public InvalidPasswordException(string message) : base(message, "InvalidPassword")
    {
    }
}
