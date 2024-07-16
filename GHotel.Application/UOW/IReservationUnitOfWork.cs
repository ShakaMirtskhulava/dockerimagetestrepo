using GHotel.Application.Repositories;

namespace GHotel.Application.UOW;

public interface IReservationUnitOfWork : IUnitOfWork
{
    IReservationRepository ReservationRepository { get; }
    IRoomRepository RoomRepository { get; }
    IUserRepository UserRepository { get; }
}
