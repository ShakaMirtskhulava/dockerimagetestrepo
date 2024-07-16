using GHotel.Domain.Enums;

namespace GHotel.Application.Models.Reservation;

#pragma warning disable CS8618
public class ReservationRequestModel
{
    public DateTime CheckInDateUtc { get; set; }
    public DateTime CheckOutDateUtc { get; set; }
    public int NumberOfGuests { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public int NumberOfNights => (CheckOutDateUtc - CheckInDateUtc).Days;

    public string UserId { get; set; }
    public string RoomId { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Card;
    public PaymentStatuss PaymentStatuss { get; set; } = PaymentStatuss.Pending;
}
