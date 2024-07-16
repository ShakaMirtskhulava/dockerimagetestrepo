using GHotel.Application.Repositories;
using GHotel.Application.UOW;
using GHotel.Persistance.Context;

namespace GHotel.Infrastructure.UOW;

public class UserUnitOfWork : UnitOfWork, IUserUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public IRoleRepository RoleRepository { get; }

    public UserUnitOfWork(GHotelDBContext dbContext, IUserRepository userRepository, IRoleRepository roleRepository) : base(dbContext)
    {
        UserRepository = userRepository;
        RoleRepository = roleRepository;
    }
}
