using GHotel.Application.Exceptions;

namespace GHotel.Infrastructure.Exceptions;

public class ImageFileNotFoundException : NotFoundException
{
    public ImageFileNotFoundException(string message) : base(message,"ImageFileNotFoundException")
    {
    }
}
