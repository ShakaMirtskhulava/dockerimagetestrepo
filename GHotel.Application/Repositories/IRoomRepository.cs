using GHotel.Domain.Entities;

namespace GHotel.Application.Repositories;
public interface IRoomRepository
{
    Task<Room?> Get(string id, CancellationToken cancellationToken);
    Task<IEnumerable<Room>> GetAllWithImages(CancellationToken cancellationToken);
    Task<Room?> GetWithImages(string id, CancellationToken cancellationToken);
    Task<Room?> Update(Room updatedRoom, CancellationToken cancellationToken);
}
