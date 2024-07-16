using GHotel.Application.UOW;
using GHotel.Application.Utilities;

namespace GHotel.Application.Services.ImageService;

public class ImageService : IImageService
{
    private readonly IImageUtility _imageUtility;
    private readonly IImageUnitOfWork _imageUnitOfWork;

    public ImageService(IImageUtility imageUtility, IImageUnitOfWork imageUnitOfWork)
    {
        _imageUtility = imageUtility;
        _imageUnitOfWork = imageUnitOfWork;
    }

    public async Task<Stream> GetFile(string url, CancellationToken cancellationToken)
    {
        return await _imageUtility.ReadImageFromFile(url, cancellationToken).ConfigureAwait(false);
    }

    //public async Task<bool> Delete(string id, CancellationToken cancellationToken)
    //{
    //    var targetImage = await _imageUnitOfWork.ImageRepository.Get(id, cancellationToken).ConfigureAwait(false);
    //    if (targetImage is null)
    //        return false;
    //    _imageUtility.DeleteImageFile(targetImage.Url);
    //    _imageUnitOfWork.ImageRepository.RemoveRange(targetImage);
    //    await _imageUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    //    return true;
    //}

}
