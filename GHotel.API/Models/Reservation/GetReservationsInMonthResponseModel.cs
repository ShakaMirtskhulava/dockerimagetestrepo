namespace GHotel.API.Models.Reservation;

#pragma warning disable CS8618

public class GetReservationsInMonthResponseModel
{
    public DateTime CheckInDateUtc { get; set; }
    public DateTime CheckOutDateUtc { get; set; }
    public string Identifier { get; set; }
}
