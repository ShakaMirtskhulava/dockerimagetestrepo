using GHotel.Application.Models.Image;

namespace GHotel.Application.Utilities;

public interface IImageUtility
{
    Task<string> SaveImageToFile(ImageRequestModel imageRequestModel, CancellationToken cancellationToken);
    Task<Stream> ReadImageFromFile(string imageFileUrl, CancellationToken cancellationToken);
    void DeleteImageFile(string imageFileUrl);
    void DeleteImageFileRange(List<string> imageFileUrls);
}
