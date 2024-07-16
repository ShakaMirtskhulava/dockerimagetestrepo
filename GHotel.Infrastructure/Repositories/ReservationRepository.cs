using System.Data;
using System.Linq.Expressions;
using GHotel.Application.Repositories;
using GHotel.Domain.Entities;
using GHotel.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace GHotel.Infrastructure.Repositories;

public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(GHotelDBContext dbContext) : base(dbContext)
    {
    }

    public async new Task<Reservation?> Update(Reservation reservation, CancellationToken cancellationToken)
    {
        var updatedentity = await base.Update(reservation,cancellationToken).ConfigureAwait(false);
        _dbContext.Entry(reservation).Property(x => x.Identifier).IsModified = false;
        return updatedentity;
    }

    public async Task<Reservation?> GetByIdentifier(int identifier,CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(re => re.User)
            .Include(re => re.Room)
            .Include(re => re.Payment)
            .FirstOrDefaultAsync(re => re.Identifier == identifier, cancellationToken).ConfigureAwait(false);
    }

    public async new Task<bool> Any(Expression<Func<Reservation,bool>> predicate,CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
    }

    public async new Task<IEnumerable<Reservation>> GetAll(Expression<Func<Reservation,bool>> predicate,CancellationToken cancellationToken)
    {
        return await _dbSet
                        .Include(re => re.Room)
                        .Include(re => re.User)
                        .Where(predicate)
                        .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async new Task<Reservation> Add(Reservation reservation, CancellationToken cancellationToken)
    {
        return await base.Add(reservation, cancellationToken).ConfigureAwait(false);
    }

}
