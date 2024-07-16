namespace GHotel.Application.Exceptions;

public class UserWithEmailNotFoundException : NotFoundException
{
    public UserWithEmailNotFoundException(string message) : base(message,"UserWithEmailNotFound")
    {
    }
}
