using GHotel.Application.Repositories;
using GHotel.Application.UOW;
using GHotel.Persistance.Context;

namespace GHotel.Infrastructure.UOW;

public class RoomUnitOfWork : UnitOfWork, IRoomUnitOfWork
{
    public IRoomRepository RoomRepository { get; }
    public IImageRepository ImageRepository { get; }

    public RoomUnitOfWork(GHotelDBContext dbContext, IRoomRepository roomRepository, IImageRepository imageRepository) : base(dbContext)
    {
        RoomRepository = roomRepository;
        ImageRepository = imageRepository;
    }
}
