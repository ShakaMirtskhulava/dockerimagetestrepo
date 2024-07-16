using System.Linq.Expressions;
using GHotel.Application.Repositories;
using GHotel.Domain.Entities;
using GHotel.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace GHotel.Infrastructure.Repositories;

public class RoleRepository : BaseRepository<MyRole>, IRoleRepository
{
    public RoleRepository(GHotelDBContext dbContext) : base(dbContext)
    {
    }

    public async new Task<MyRole?> Get(Expression<Func<MyRole,bool>> predicate,CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
    }

}
