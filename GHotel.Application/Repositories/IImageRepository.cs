using GHotel.Domain.Entities;

namespace GHotel.Application.Repositories;

public interface IImageRepository
{
    Task<MyImage?> Get(string id, CancellationToken cancellationToken);
    void RemoveRange(IEnumerable<MyImage> images);
}
