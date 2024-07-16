using GHotel.Application.Repositories;

namespace GHotel.Application.UOW;

public interface IImageUnitOfWork : IUnitOfWork
{
    public IImageRepository ImageRepository { get; }
}
