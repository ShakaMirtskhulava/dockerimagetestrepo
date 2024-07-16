namespace GHotel.API.Models.Room;

#pragma warning disable CS8618

public class UpdateRoomModel
{
    public string Id { get; set; }
    public IFormFileCollection Images { get; set; }
}
