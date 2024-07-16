using GHotel.Application.Repositories;
using GHotel.Domain.Entities;
using GHotel.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace GHotel.Infrastructure.Repositories;

public class RoomRepository : BaseRepository<Room>, IRoomRepository
{
    public RoomRepository(GHotelDBContext dbContext) : base(dbContext)
    {
    }

    public async Task<Room?> Get(string id,CancellationToken cancellationToken)
    {
        return await base.Get(cancellationToken, id).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Room>> GetAllWithImages(CancellationToken cancellationToken)
    {
        return await _dbSet.Include(x => x.Images).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<Room?> GetWithImages(string id, CancellationToken cancellationToken)
    {
        return await _dbSet.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
    }

    public async new Task<Room?> Update(Room updatedRoom,CancellationToken cancellationToken)
    {
        return await base.Update(updatedRoom, cancellationToken).ConfigureAwait(false);
    }

}
