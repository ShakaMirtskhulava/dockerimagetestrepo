using GHotel.Application.Models.Image;

namespace GHotel.Application.Models.Room;

#pragma warning disable CS8618

public class RoomUpdateModel
{
    public string Id { get; set; }
    public List<ImageRequestModel> Images { get; set; }
}
