using GHotel.Application.Repositories;

namespace GHotel.Application.UOW;

public interface IUserUnitOfWork : IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public IRoleRepository RoleRepository { get; }
}
