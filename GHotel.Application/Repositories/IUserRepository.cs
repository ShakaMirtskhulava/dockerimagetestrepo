using System.Linq.Expressions;
using GHotel.Domain.Entities;

namespace GHotel.Application.Repositories;

public interface IUserRepository
{
    Task<MyUser> Add(MyUser user, CancellationToken cancellationToken);
    Task<MyUser?> Get(Expression<Func<MyUser, bool>> predicate, CancellationToken cancellationToken);
    Task<MyUser?> Get(string id, CancellationToken cancellationToken);
    Task<bool> Any(Expression<Func<MyUser, bool>> predicate, CancellationToken cancellationToken);
    Task<MyUser?> GetByEmail(string email, CancellationToken cancellationToken);
    Task<MyUser?> Update(MyUser user, CancellationToken cancellationToken);
}
