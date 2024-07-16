using GHotel.Application.Repositories;

namespace GHotel.Application.UOW;

public interface IRoomUnitOfWork : IUnitOfWork
{
    IRoomRepository RoomRepository { get; }
    IImageRepository ImageRepository { get; }
}

