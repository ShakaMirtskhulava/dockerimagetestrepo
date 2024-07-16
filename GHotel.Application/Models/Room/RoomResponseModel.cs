using GHotel.Application.Models.Image;
using GHotel.Domain.Enums;

namespace GHotel.Application.Models.Room;

#pragma warning disable CS8618
public class RoomResponseModel
{
    public string Id { get; set; }
    public RoomType Type { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public Currency PricePerNightCurrency { get; set; }

    public List<ImageResponseModel>? Images { get; set; }
}
