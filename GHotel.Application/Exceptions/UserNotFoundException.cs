namespace GHotel.Application.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string message) : base(message,"UserNotFound")
    {
    }
}
