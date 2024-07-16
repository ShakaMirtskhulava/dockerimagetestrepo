using GHotel.Domain.Enums;

namespace GHotel.Application.Models.Room;

#pragma warning disable CS8618

public class GetRoomRequestModel
{
    public string Id { get; set; }
    public Currency Currency { get; set; }
}
