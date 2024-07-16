namespace GHotel.Application.Services.ImageService;

public interface IImageService
{
    Task<Stream> GetFile(string url, CancellationToken cancellationToken);
}
