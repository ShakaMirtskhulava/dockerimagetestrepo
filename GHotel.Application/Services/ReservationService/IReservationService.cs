using GHotel.Application.Models.Reservation;

namespace GHotel.Application.Services.ReservationService;

public interface IReservationService
{
    Task<ReservationResponseModel> Get(string identifier, string userId, CancellationToken cancellationToken);
    Task<IEnumerable<ReservationResponseModel>> GetUserReservations(string userId, CancellationToken cancellationToken);
    Task<CreateReservationResponseModel> CreateReservation(ReservationRequestModel reservationRequestModel, CancellationToken cancellationToken);
    Task<ReservationResponseModel> ApproveReservation(string identifier, CancellationToken cancellationToken);
    Task<ReservationResponseModel> CancelReservation(string identifier, CancellationToken cancellationToken);
    Task<ReservationResponseModel> CancelAuthorizedReservation(string identifier, string userId, CancellationToken cancellationToken);
    Task<IEnumerable<ReservationResponseModel>> GetAllInMonth(string roomId, int year, int month, CancellationToken cancellationToken);
    Task CompleteReservations(CancellationToken cancellationToken);
}
