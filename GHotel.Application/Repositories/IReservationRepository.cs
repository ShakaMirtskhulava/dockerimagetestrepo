using GHotel.Domain.Entities;
using System.Linq.Expressions;

namespace GHotel.Application.Repositories;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdentifier(int identifier, CancellationToken cancellationToken);
    Task<Reservation?> Update(Reservation reservation, CancellationToken cancellationToken);
    Task<bool> Any(Expression<Func<Reservation, bool>> predicate, CancellationToken cancellationToken);
    Task<Reservation> Add(Reservation reservation, CancellationToken cancellationToken);
    Task<IEnumerable<Reservation>> GetAll(Expression<Func<Reservation, bool>> predicate, CancellationToken cancellationToken);
}
