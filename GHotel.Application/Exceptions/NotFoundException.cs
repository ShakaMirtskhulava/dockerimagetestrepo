namespace GHotel.Application.Exceptions;

public class NotFoundException : Exception
{
    public string Code { get;} = "NotFound";

    public NotFoundException(string message,string code) : base(message)
    {
        Code = code;
    }
}
