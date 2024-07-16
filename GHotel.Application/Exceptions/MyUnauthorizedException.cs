namespace GHotel.Application.Exceptions;

public class MyUnauthorizedException : Exception
{
    public string Code { get; }

    public MyUnauthorizedException(string message,string code) : base(message)
    {
        Code = code;
    }
}
