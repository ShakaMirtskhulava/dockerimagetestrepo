namespace GHotel.Application.Exceptions;

public class OperationFailedException : Exception
{
    public string Code { get;} = "OperationFailed";

    public OperationFailedException(string message,string? code = null) : base(message)
    {
        if(code != null)
            Code = code;
    }
}
