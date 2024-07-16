using GHotel.Domain.Enums;

namespace GHotel.API.Models.Reservation;

#pragma warning disable CS8618
public class ReservationCreateModel
{
    public string RoomId { get; set; }
    public DateTime CheckInDateUtc { get; set; }
    public DateTime CheckOutDateUtc { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public int NumberOfGuests { get; set; }
}
