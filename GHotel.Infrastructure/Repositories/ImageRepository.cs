using GHotel.Application.Repositories;
using GHotel.Domain.Entities;
using GHotel.Persistance.Context;

namespace GHotel.Infrastructure.Repositories;

public class ImageRepository : BaseRepository<MyImage>, IImageRepository
{
    public ImageRepository(GHotelDBContext dbContext) : base(dbContext)
    {
    }

    public async Task<MyImage?> Get(string id,CancellationToken cancellationToken)
    {
        return await base.Get(cancellationToken, id).ConfigureAwait(false);
    }

    public new void RemoveRange(IEnumerable<MyImage> images)
    {
        _dbSet.RemoveRange(images);
    }

}
