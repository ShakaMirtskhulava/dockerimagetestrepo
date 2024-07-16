using GHotel.Domain.Enums;

namespace GHotel.Domain.Entities;

#pragma warning disable CS8618
public class Room : Entity
{
    public string Description { get; set; }
    public RoomType Type { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public Currency PricePerNightCurrency { get; set; }

    public ICollection<MyImage> Images { get; set; }
    public ICollection<Reservation>? Reservations { get; set; }
}
