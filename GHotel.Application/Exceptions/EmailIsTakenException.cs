namespace GHotel.Application.Exceptions;

public class EmailIsTakenException : OperationFailedException
{
    public EmailIsTakenException(string message) : base(message, "EmailIsTaken")
    {
    }
}
