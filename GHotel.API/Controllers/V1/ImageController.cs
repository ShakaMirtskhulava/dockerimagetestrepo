using Asp.Versioning;
using GHotel.API.Infrastructure.Error;
using GHotel.Application.Services.ImageService;
using Microsoft.AspNetCore.Mvc;

namespace GHotel.API.Controllers.V1;

[ApiController]
[Route("v{version:apiversion}/[controller]")]
[ApiVersion(1)]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    /// <summary>
    /// Get's the image as a file
    /// </summary>
    /// <param name="url"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Newly created user</returns>
    /// <response code="200">if the image was returned</response>
    /// <response code="404">if the image was not found</response>
    /// <response code="500">if there was some problem on the server</response>
    [HttpGet("{url}")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(APIError), 404)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<IActionResult> GetImage(string url, CancellationToken cancellationToken)
    {
        var imageStream = await _imageService.GetFile(url, cancellationToken).ConfigureAwait(false);
        var imageExtension = Path.GetExtension(url);

        return File(imageStream, $"image/{imageExtension}");
    }

}
