using GHotel.Application.Repositories;
using GHotel.Application.UOW;
using GHotel.Persistance.Context;

namespace GHotel.Infrastructure.UOW;

public class ReservationUnitOfWork : UnitOfWork, IReservationUnitOfWork
{
    public IReservationRepository ReservationRepository { get; }
    public IRoomRepository RoomRepository { get; }
    public IUserRepository UserRepository { get; }

    public ReservationUnitOfWork(GHotelDBContext gHotelDBContext, IReservationRepository reservationRepository, IRoomRepository roomRepository, IUserRepository userRepository) : base(gHotelDBContext)
    {
        ReservationRepository = reservationRepository;
        RoomRepository = roomRepository;
        UserRepository = userRepository;
    }
}
