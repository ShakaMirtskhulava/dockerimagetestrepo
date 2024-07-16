namespace GHotel.Application.Exceptions;

public class RoomAlreadyBookedException : Exception
{
    public string Code { get; } = "RoomAlreadyBooked";

    public RoomAlreadyBookedException(string message) : base(message)
    {
    }
}
