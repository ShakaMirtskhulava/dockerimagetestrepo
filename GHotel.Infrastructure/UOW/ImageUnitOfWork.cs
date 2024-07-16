using GHotel.Application.Repositories;
using GHotel.Application.UOW;
using GHotel.Persistance.Context;

namespace GHotel.Infrastructure.UOW;

public class ImageUnitOfWork : UnitOfWork, IImageUnitOfWork
{
    public IImageRepository ImageRepository { get; }

    public ImageUnitOfWork(GHotelDBContext gHotelDBContext,IImageRepository imageRepository) : base(gHotelDBContext)
    {
        ImageRepository = imageRepository;
    }
}
