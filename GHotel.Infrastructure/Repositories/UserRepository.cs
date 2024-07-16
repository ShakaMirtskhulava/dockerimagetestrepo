using System.Linq.Expressions;
using GHotel.Application.Repositories;
using GHotel.Domain.Entities;
using GHotel.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace GHotel.Infrastructure.Repositories;

public class UserRepository : BaseRepository<MyUser>, IUserRepository
{
    public UserRepository(GHotelDBContext dbContext) : base(dbContext)
    {
    }

    public async new Task<MyUser> Add(MyUser user,CancellationToken cancellationToken)
    {
        return await base.Add(user, cancellationToken).ConfigureAwait(false);
    }

    public async Task<MyUser?> Update(MyUser user,CancellationToken cancellationToken)
    {
        return await base.Update(user, cancellationToken).ConfigureAwait(false);
    }

    public async Task<MyUser?> Get(string id,CancellationToken cancellationToken)
    {
        return await base.Get(cancellationToken, id).ConfigureAwait(false);
    }

    public async new Task<MyUser?> Get(Expression<Func<MyUser, bool>> predicate, CancellationToken cancellationToken)
    {
        return await base.Get(predicate, cancellationToken).ConfigureAwait(false);
    }

    public async Task<MyUser?> GetByEmail(string email,CancellationToken cancellationToken)
    {
        return await _dbSet.Include(us => us.Roles).FirstOrDefaultAsync(us => us.Email == email && us.EmailConfirmed, cancellationToken).ConfigureAwait(false);
    }

    public async new Task<bool> Any(Expression<Func<MyUser, bool>> predicate, CancellationToken cancellationToken)
    {
        return await base.Any(predicate, cancellationToken).ConfigureAwait(false);
    }

}
