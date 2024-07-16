using GHotel.Application.Models.Room;
using GHotel.Application.Models.User;
using GHotel.Domain.Enums;

namespace GHotel.Application.Models.Reservation;

#pragma warning disable CS8618

public class ReservationResponseModel
{
    public DateTime CheckInDateUtc { get; set; }
    public DateTime CheckOutDateUtc { get; set; }
    public int NumberOfGuests { get; set; }
    public ReservationStatus Status { get; set; }
    public string Identifier { get; set; }

    public UserResponseModel User { get; set; }
    public RoomResponseModel Room { get; set; }
}
