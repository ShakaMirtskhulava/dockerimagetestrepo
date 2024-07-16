using GHotel.Domain.Enums;

namespace GHotel.Domain.Entities;

#pragma warning disable CS8618
public class Reservation : Entity
{
    public int Identifier { get; set; }
    public DateTime CheckInDateUtc { get; set; }
    public DateTime CheckOutDateUtc { get; set; }
    public int NumberOfGuests { get; set; }
    public ReservationStatus Status { get; set; }

    public string PaymentId { get; set; }
    public Payment Payment { get; set; }

    public string UserId { get; set; }
    public MyUser User { get; set; }

    public string RoomId { get; set; }
    public Room Room { get; set; }
}
