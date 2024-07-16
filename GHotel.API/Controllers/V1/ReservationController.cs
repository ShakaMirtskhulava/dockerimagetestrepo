using System.Security.Claims;
using Asp.Versioning;
using GHotel.API.Infrastructure.Configuration;
using GHotel.API.Infrastructure.Error;
using GHotel.API.Infrastructure.Examples;
using GHotel.API.Models.Reservation;
using GHotel.Application.Exceptions;
using GHotel.Application.Models.Reservation;
using GHotel.Application.Services.ReservationService;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;

namespace GHotel.API.Controllers.V1;

[ApiController]
[Route("v{version:apiversion}/[controller]")]
[ApiVersion(1)]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly ILogger<ReservationController> _logger;
    private readonly FrontEndConfiguration _frontEndConfiguration;

    public ReservationController(IReservationService reservationService, IOptions<FrontEndConfiguration> frontEndConfigurationOptions, ILogger<ReservationController> logger)
    {
        _reservationService = reservationService;
        _frontEndConfiguration = frontEndConfigurationOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Get's the reservation by the identifier
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">if reservation was successfully retirieved</response>
    /// <response code="404">if reservation was not found</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpGet("{identifier}")]
    [Authorize(Policy = "Authenticated")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ReservationResponseModel), 200)]
    [ProducesResponseType(typeof(APIError), 404)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<ReservationResponseModel> Get(string identifier, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var responseModel = await _reservationService.Get(identifier,userId ,cancellationToken).ConfigureAwait(false);
        return responseModel;
    }

    /// <summary>
    /// Get's all the reservations of the currently authenticated user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <response code="200">if reservations were successfully retirieved</response>
    /// <response code="404">if reservations were not found</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpGet("UserReservations")]
    [Authorize(Policy = "IsUser")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<ReservationResponseModel>), 200)]
    [ProducesResponseType(typeof(APIError), 404)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<IEnumerable<ReservationResponseModel>> GetUserReservations(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var responseModels = await _reservationService.GetUserReservations(userId, cancellationToken).ConfigureAwait(false);
        return responseModels;
    }

    /// <summary>
    /// Get the rooms reservations in the specified month
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">if reservations were successfully retirieved</response>
    /// <response code="404">if room was not found</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpGet("InMonth/{roomId}/{year}/{month}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<GetReservationsInMonthResponseModel>), 200)]
    [ProducesResponseType(typeof(APIError), 404)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<IEnumerable<GetReservationsInMonthResponseModel>> GetReservationsInMonth(string roomId, int year, int month, CancellationToken cancellationToken)
    {
        var reservationResponseModels = await _reservationService.GetAllInMonth(roomId, year, month, cancellationToken).ConfigureAwait(false);
        return reservationResponseModels.Adapt<IEnumerable<GetReservationsInMonthResponseModel>>();
    }

    /// <summary>
    /// Creates new reservation
    /// </summary>
    /// <param name="reservationCreateModel"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">if reservation was created</response>
    /// <response code="404">if room was not found</response>
    /// <response code="400">if reservation was not created</response>
    /// <response code="500">if there is problem on the server</response>
    [Authorize(Policy = "IsUser")]
    [HttpPost]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(ReservationCreateModel), typeof(ReservationCreateModelExample))]
    [ProducesResponseType(typeof(CreateReservationResponseModel), 200)]
    [ProducesResponseType(typeof(APIError), 400)]
    [ProducesResponseType(typeof(APIError), 404)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<ReservationCreateResponseModel> Create(ReservationCreateModel reservationCreateModel,CancellationToken cancellationToken)
    {
        var reservationRequestModel = reservationCreateModel.Adapt<ReservationRequestModel>();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        reservationRequestModel.UserId = userId;
        var responseModel = await _reservationService.CreateReservation(reservationRequestModel, cancellationToken).ConfigureAwait(false);
        CookieOptions options = new() { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddMinutes(3) };
        Response.Cookies.Append("ReservationIdentifier", responseModel.Identifier, options);
        _logger.LogInformation("Reservation created {@ReservationCreateResponseModel}", responseModel);
        return responseModel.Adapt<ReservationCreateResponseModel>();
    }

    /// <summary>
    /// Cancells the reservation
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">if reservation was cancelled</response>
    /// <response code="404">if reservation was not found</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpPatch("Cancel/{identifier}")]
    [Authorize(Policy = "IsUser")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ReservationCreateResponseModel), 200)]
    [ProducesResponseType(typeof(APIError), 404)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<IActionResult> Cancel(string identifier,CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var responseModel = await _reservationService.CancelAuthorizedReservation(identifier, userId, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Reservation canceled {@ReservationCreateResponseModel}", responseModel);
        return Redirect(_frontEndConfiguration.Origin + _frontEndConfiguration.ReservationCancelationPage + $"?ReservationIdentifier={responseModel.Identifier}");
    }

    [Authorize(Policy = "IsUser")]
    [HttpGet("Success")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Success([FromQuery] string token, CancellationToken cancellationToken)
    {
        try
        {
            var reservationIdentifier = Request.Cookies["ReservationIdentifier"];
            if (reservationIdentifier == null)
                throw new Exception("Reservation identifier was not found in the cookies");
            var responseModel = await _reservationService.ApproveReservation(reservationIdentifier, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Reservation approved {@ReservationCreateResponseModel}", responseModel);
            Response.Cookies.Delete("ReservationIdentifier");
            return Redirect(_frontEndConfiguration.Origin + _frontEndConfiguration.ReservationSuccessPage + $"?ReservationIdentifier={responseModel.Identifier}");
        }
        catch (RoomAlreadyBookedException)
        {
            return Redirect(_frontEndConfiguration.Origin + _frontEndConfiguration.ReservationErrorPage + "?error=RoomAlreadyBooked");
        }
    }

    [Authorize(Policy = "IsUser")]
    [HttpGet("Cancel")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Cancel(CancellationToken cancellationToken)
    {
        var reservationIdentifier = Request.Cookies["ReservationIdentifier"];
        if (reservationIdentifier == null)
            throw new Exception("Reservation identifier was not found in the cookies");
        var responseModel = await _reservationService.CancelReservation(reservationIdentifier, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Reservation canceled {@ReservationCreateResponseModel}", responseModel);
        return Redirect(_frontEndConfiguration.Origin + _frontEndConfiguration.ReservationCancelationPage + $"?ReservationIdentifier={responseModel.Identifier}");
    }

}
