using GHotel.Application.Models.Room;

namespace GHotel.Application.Services.RoomService;

public interface IRoomService
{
    Task<IEnumerable<RoomResponseModel>> GetAll(CancellationToken cancellationToken);
    Task<RoomResponseModel> Get(GetRoomRequestModel getRoomRequestModel, CancellationToken cancellationToken);
    Task<RoomResponseModel> Update(RoomUpdateModel roomUpdateModel, CancellationToken cancellationToken);
}
