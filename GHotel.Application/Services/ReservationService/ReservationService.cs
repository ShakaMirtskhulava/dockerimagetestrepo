using System.Globalization;
using GHotel.Application.Exceptions;
using GHotel.Application.Models.Payment;
using GHotel.Application.Models.Reservation;
using GHotel.Application.Services.PaymentService;
using GHotel.Application.UOW;
using GHotel.Application.Utilities;
using GHotel.Domain.Entities;
using GHotel.Domain.Enums;
using Mapster;

namespace GHotel.Application.Services.ReservationService;

public class ReservationService : IReservationService
{
    private readonly IReservationUnitOfWork _reservationUnitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IIdHasherUtility _idHasherUtility;
    private readonly ILockUtility _lockUtility;

    public ReservationService(IReservationUnitOfWork reservationUnitOfWork, IPaymentService paymentService, IIdHasherUtility idHasherUtility, ILockUtility lockUtility)
    {
        _reservationUnitOfWork = reservationUnitOfWork;
        _paymentService = paymentService;
        _idHasherUtility = idHasherUtility;
        _lockUtility = lockUtility;
    }

    public async Task<ReservationResponseModel> Get(string identifier, string userId, CancellationToken cancellationToken)
    {
        var targetReservation = await _reservationUnitOfWork.ReservationRepository.GetByIdentifier(_idHasherUtility.Decode(identifier), cancellationToken).ConfigureAwait(false);
        if (targetReservation == null)
            throw new NotFoundException($"Reservation with identifier: {identifier} was not found", "ReservationNotFound");
        if (targetReservation.UserId != userId)
            throw new UnauthorizedAccessException("User is not authorized to access this reservation");

        var result = targetReservation.Adapt<ReservationResponseModel>();
        result.Identifier = _idHasherUtility.Encode(targetReservation.Identifier);
        return result;
    }

    public async Task<IEnumerable<ReservationResponseModel>> GetUserReservations(string userId, CancellationToken cancellationToken)
    {
        var userExists = await _reservationUnitOfWork.UserRepository.Any(us => us.Id == userId,cancellationToken).ConfigureAwait(false);
        if (!userExists)
            throw new UserNotFoundException($"User with id: {userId} was not found");
        var reservations = await _reservationUnitOfWork.ReservationRepository.GetAll(re => re.UserId == userId, cancellationToken).ConfigureAwait(false);
        var responseModels = reservations.Adapt<IEnumerable<ReservationResponseModel>>();

        responseModels = responseModels.Select(res =>
        {
            res.Identifier = _idHasherUtility.Encode(int.Parse(res.Identifier, CultureInfo.CurrentCulture));
            return res;
        });
        return responseModels;
    }

    public async Task<CreateReservationResponseModel> CreateReservation(ReservationRequestModel reservationRequestModel, CancellationToken cancellationToken)
    {
        var targetRoom = await _reservationUnitOfWork.RoomRepository.Get(reservationRequestModel.RoomId, cancellationToken).ConfigureAwait(false);
        if (targetRoom == null)
            throw new NotFoundException($"Room with id: {reservationRequestModel.RoomId} was not found", "RoomNotFound");
        var booked = await IsRoomBooked(reservationRequestModel.RoomId, reservationRequestModel.CheckInDateUtc, reservationRequestModel.CheckOutDateUtc, cancellationToken)
                            .ConfigureAwait(false);
        if (booked)
            throw new RoomAlreadyBookedException($"Attempt to book room with id: {reservationRequestModel.RoomId} in the booked interval");
        var paymentRequestModel = new PaymentRequestModel
        {
            Amount = targetRoom.PricePerNight * reservationRequestModel.NumberOfNights,
            Method = reservationRequestModel.PaymentMethod,
            Currency = Currency.USD
        };
        var paymentResponseModel = await _paymentService.CreatePayment(paymentRequestModel, cancellationToken).ConfigureAwait(false);
        var reservation = reservationRequestModel.Adapt<Reservation>();
        reservation.PaymentId = paymentResponseModel.Id;
        reservation.CheckInDateUtc = reservation.CheckInDateUtc.ToUniversalTime();
        reservation.CheckOutDateUtc = reservation.CheckOutDateUtc.ToUniversalTime();

        var newReservation = await _reservationUnitOfWork.ReservationRepository.Add(reservation, cancellationToken).ConfigureAwait(false);
        await _reservationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new CreateReservationResponseModel
        {
            ApproveLink = paymentResponseModel.ApproveLink,
            Identifier = _idHasherUtility.Encode(newReservation.Identifier)
        };
    }

    public async Task<ReservationResponseModel> ApproveReservation(string identifier, CancellationToken cancellationToken)
    {
        var targetReservation = await _reservationUnitOfWork.ReservationRepository.GetByIdentifier(_idHasherUtility.Decode(identifier), cancellationToken).ConfigureAwait(false);
        if(targetReservation == null)
            throw new NotFoundException($"Reservation with identifier: {identifier} was not found", "ReservationNotFound");
        if (targetReservation.Status != ReservationStatus.Pending)
            throw new InvalidOperationException("Reservation is not pending");

        await _lockUtility.Lock(targetReservation.RoomId).ConfigureAwait(false);
        try
        {
            var booked = await IsRoomBooked(targetReservation.RoomId, targetReservation.CheckInDateUtc, targetReservation.CheckOutDateUtc, cancellationToken).ConfigureAwait(false);
            if (booked)
            {
                await CancelReservation(identifier, cancellationToken).ConfigureAwait(false);
                throw new RoomAlreadyBookedException($"Attempt to book room with id: {targetReservation.RoomId} in the booked interval");
            }

            await _paymentService.CheckoutAuthorizePayment(targetReservation.PaymentId, cancellationToken).ConfigureAwait(false);
            targetReservation.Status = ReservationStatus.Confirmed;
            await _reservationUnitOfWork.ReservationRepository.Update(targetReservation, cancellationToken).ConfigureAwait(false);
            await _reservationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _lockUtility.Release(targetReservation.RoomId);
        }

        var result = targetReservation.Adapt<ReservationResponseModel>();
        result.Identifier = _idHasherUtility.Encode(targetReservation.Identifier);
        return result;
    }

    public async Task CompleteReservations(CancellationToken cancellationToken)
    {
        var currentDate = DateTime.UtcNow;
        var reservationsToComplete = await _reservationUnitOfWork.ReservationRepository.GetAll(
                                                re =>
                                                re.Status == ReservationStatus.Confirmed
                                                &&
                                                re.CheckInDateUtc <= currentDate
                                                , cancellationToken).ConfigureAwait(false);
        foreach (var reservation in reservationsToComplete)
        {
            await _paymentService.CaptureAuthorizePayment(reservation.PaymentId, cancellationToken).ConfigureAwait(false);
            reservation.Status = ReservationStatus.Completed;
            await _reservationUnitOfWork.ReservationRepository.Update(reservation, cancellationToken).ConfigureAwait(false);
        }
        await _reservationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<ReservationResponseModel> CancelReservation(string identifier, CancellationToken cancellationToken)
    {
        var targetReservation = await _reservationUnitOfWork.ReservationRepository.GetByIdentifier(_idHasherUtility.Decode(identifier), cancellationToken).ConfigureAwait(false);
        if (targetReservation == null)
            throw new NotFoundException($"Reservation with identifier: {identifier} was not found", "ReservationNotFound");
        if (targetReservation.Status != ReservationStatus.Pending)
            throw new InvalidOperationException("Reservation is not pending");

        targetReservation.Status = ReservationStatus.Cancelled;
        await _reservationUnitOfWork.ReservationRepository.Update(targetReservation, cancellationToken).ConfigureAwait(false);
        await _reservationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var result = targetReservation.Adapt<ReservationResponseModel>();
        result.Identifier = _idHasherUtility.Encode(targetReservation.Identifier);
        return result;
    }

    public async Task<ReservationResponseModel> CancelAuthorizedReservation(string identifier,string userId, CancellationToken cancellationToken)
    {
        var targetReservation = await _reservationUnitOfWork.ReservationRepository.GetByIdentifier(_idHasherUtility.Decode(identifier), cancellationToken).ConfigureAwait(false);
        if (targetReservation == null)
            throw new NotFoundException($"Reservation with identifier: {identifier} was not found", "ReservationNotFound");
        if (targetReservation.Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Reservation is not pending");
        if(targetReservation.UserId != userId)
            throw new MyUnauthorizedException("User is not authorized to cancel this reservation","NotReservationOwner");

        decimal fee = 0;
        if (targetReservation.CheckInDateUtc >= DateTime.UtcNow.AddDays(-1))
            fee = targetReservation.Payment.Amount * 0.4m;
        await _paymentService.CancelPayment(targetReservation.PaymentId, fee, cancellationToken).ConfigureAwait(false);

        targetReservation.Status = ReservationStatus.Cancelled;
        await _reservationUnitOfWork.ReservationRepository.Update(targetReservation, cancellationToken).ConfigureAwait(false);
        await _reservationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var result = targetReservation.Adapt<ReservationResponseModel>();
        result.Identifier = _idHasherUtility.Encode(targetReservation.Identifier);
        return result;
    }

    public async Task<IEnumerable<ReservationResponseModel>> GetAllInMonth(string roomId,int year,int month, CancellationToken cancellationToken)
    {
        if (month < 1 || month > 12)
            throw new InvalidOperationException($"Attempt to get the reservations of the room with id: {roomId} in the month: {month},but month must be between 1 and 12");

        var reservationsInMonth = await _reservationUnitOfWork.ReservationRepository.GetAll(res =>
                                            res.RoomId == roomId
                                            &&
                                            (
                                                (res.CheckInDateUtc.Month == month && res.CheckInDateUtc.Year == year && res.Status == ReservationStatus.Confirmed)
                                                ||
                                                (res.CheckOutDateUtc.Month == month && res.CheckOutDateUtc.Year == year && res.Status == ReservationStatus.Confirmed)
                                            )
                                        , cancellationToken).ConfigureAwait(false);
        var responseModels = reservationsInMonth.Adapt<IEnumerable<ReservationResponseModel>>();
        responseModels = responseModels.Select(res =>
        {
            res.Identifier = _idHasherUtility.Encode(int.Parse(res.Identifier,CultureInfo.CurrentCulture));
            return res;
        });
        return responseModels;
    }

    private async Task<bool> IsRoomBooked(string roomId,DateTime checkInDate,DateTime checkOutDate,CancellationToken cancellationToken)
    {
        return await _reservationUnitOfWork.ReservationRepository
                            .Any(
                                re => re.RoomId == roomId
                                &&
                                (re.Status == ReservationStatus.Confirmed || re.Status == ReservationStatus.Completed)
                                &&
                                !(checkInDate >= re.CheckOutDateUtc || checkOutDate <= re.CheckInDateUtc)
                            , cancellationToken).ConfigureAwait(false);
    }

}
