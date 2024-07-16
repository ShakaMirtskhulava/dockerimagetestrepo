using System.Linq.Expressions;
using GHotel.Domain.Entities;

namespace GHotel.Application.Repositories;

public interface IRoleRepository
{
    Task<MyRole?> Get(Expression<Func<MyRole, bool>> predicate, CancellationToken cancellationToken);
}
