using GHotel.API.Models.Reservation;
using GHotel.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace GHotel.API.Infrastructure.Examples;

public class ReservationCreateModelExample : IExamplesProvider<ReservationCreateModel>
{
    public ReservationCreateModel GetExamples()
    {
        return new ReservationCreateModel
        {
            RoomId = "id1",
            CheckInDateUtc = DateTime.Now,
            CheckOutDateUtc = DateTime.Now.AddDays(1),
            NumberOfGuests = 2,
            PaymentMethod = PaymentMethod.Card
        };
    }
}
