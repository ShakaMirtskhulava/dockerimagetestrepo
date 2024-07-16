using Asp.Versioning;
using GHotel.API.Infrastructure.Error;
using GHotel.API.Models.Room;
using GHotel.Application.Models.Image;
using GHotel.Application.Models.Room;
using GHotel.Application.Services.RoomService;
using GHotel.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace GHotel.API.Controllers.V1;

[ApiController]
[Route("v{version:apiversion}/[controller]")]
[ApiVersion(1)]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomController> _logger;
    private readonly IDistributedCache _cache;

    public RoomController(IRoomService roomService, ILogger<RoomController> logger, IDistributedCache cache)
    {
        _roomService = roomService;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Get's the room by the id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="currency"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">if room was retirieved</response>
    /// <response code="404">if room was not found</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpGet("{id}/{currency}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomResponseModel), 200)]
    [ProducesResponseType(typeof(APIError), 404)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<RoomResponseModel> Get(string id,Currency currency, CancellationToken cancellationToken)
    {
        var cacheKey = $"Room_{id}_{currency}";
        RoomResponseModel roomResponseModel;
        var cachedRoom = await _cache.GetStringAsync(cacheKey, cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(cachedRoom))
        {
            roomResponseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<RoomResponseModel>(cachedRoom);
        }
        else
        {
            var request = new GetRoomRequestModel() { Id = id, Currency = currency };
            roomResponseModel = await _roomService.Get(request, cancellationToken).ConfigureAwait(false);
            var serializedRoom = Newtonsoft.Json.JsonConvert.SerializeObject(roomResponseModel);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            };
            await _cache.SetStringAsync(cacheKey, serializedRoom, options, cancellationToken).ConfigureAwait(false);
        }

        return roomResponseModel;
    }

    /// <summary>
    /// Get all the existing rooms
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <response code="200">if rooms where retirieved</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<RoomResponseModel>), 200)]
    [ProducesResponseType(typeof(APIError),500)]
    public async Task<IEnumerable<RoomResponseModel>> GetAll(CancellationToken cancellationToken)
    {
        var roomResponseModels = await _roomService.GetAll(cancellationToken).ConfigureAwait(false);
        return roomResponseModels;
    }

    /// <summary>
    /// Updates and existing room
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="updateRoomModel"></param>
    /// <response code="200">if rooms was updated successfully</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpPut]
    [Authorize(Policy = "IsAdmin")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomResponseModel), 200)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<RoomResponseModel> Update([FromForm]UpdateRoomModel updateRoomModel,CancellationToken cancellationToken)
    {
        var roomUpdateModel = new RoomUpdateModel { Id = updateRoomModel.Id, Images = new()};
        foreach (var image in updateRoomModel.Images)
        {
            var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream,cancellationToken).ConfigureAwait(false);
            roomUpdateModel.Images.Add(new ImageRequestModel { Data = memoryStream.ToArray(), FileExtension = image.FileName.Split('.').Last() });
        }
        var roomResponseModel = await _roomService.Update(roomUpdateModel, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Room Updated {@roomResponseModel}", roomResponseModel);

        return roomResponseModel;
    }

}
