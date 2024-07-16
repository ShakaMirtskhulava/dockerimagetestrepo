using GHotel.Application.Exceptions;
using GHotel.Application.Models.Room;
using GHotel.Application.UOW;
using GHotel.Application.Utilities;
using GHotel.Domain.Entities;
using GHotel.Domain.Enums;
using Mapster;

namespace GHotel.Application.Services.RoomService;

public class RoomService : IRoomService
{
    private readonly IRoomUnitOfWork _roomUnitOfWork;
    private readonly IImageUtility _imageUtility;
    private readonly ICurrencyConvertionUtility _currencyConvertionUtility;

    public RoomService(IRoomUnitOfWork roomUnitOfWork, IImageUtility imageUtility, ICurrencyConvertionUtility currencyConvertionUtility)
    {
        _roomUnitOfWork = roomUnitOfWork;
        _imageUtility = imageUtility;
        _currencyConvertionUtility = currencyConvertionUtility;
    }

    public async Task<RoomResponseModel> Get(GetRoomRequestModel getRoomRequestModel,CancellationToken cancellationToken)
    {
        var targetRoom = await _roomUnitOfWork.RoomRepository.GetWithImages(getRoomRequestModel.Id, cancellationToken).ConfigureAwait(false);
        if (targetRoom == null)
            throw new NotFoundException($"Couldn't found the Room with Id: {getRoomRequestModel.Id}","RoomNotFound");
        var response = targetRoom.Adapt<RoomResponseModel>();
        response.PricePerNight = await _currencyConvertionUtility.ConvertCurrency(targetRoom.PricePerNight, Currency.USD, getRoomRequestModel.Currency,cancellationToken)
                                        .ConfigureAwait(false);
        response.PricePerNightCurrency = getRoomRequestModel.Currency;
        return response;
    }

    public async Task<IEnumerable<RoomResponseModel>> GetAll(CancellationToken cancellationToken)
    {
        var rooms = await _roomUnitOfWork.RoomRepository.GetAllWithImages(cancellationToken).ConfigureAwait(false);
        return rooms.Adapt<IEnumerable<RoomResponseModel>>();
    }

    public async Task<RoomResponseModel> Update(RoomUpdateModel roomUpdateModel,CancellationToken cancellationToken)
    {
        var targetRoom = await _roomUnitOfWork.RoomRepository.GetWithImages(roomUpdateModel.Id, cancellationToken).ConfigureAwait(false);
        if (targetRoom == null)
            throw new NotFoundException($"Couldn't found the Room with Id: {roomUpdateModel.Id}","RoomNotFound");
        if(targetRoom.Images != null && targetRoom.Images.Count > 0)
        {
            var imageUrls = targetRoom.Images.Select(im => im.Url).ToList();
            _roomUnitOfWork.ImageRepository.RemoveRange(targetRoom.Images);
            await _roomUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _imageUtility.DeleteImageFileRange(imageUrls);
        }
        if(roomUpdateModel.Images == null)
            roomUpdateModel.Images = new();
        foreach (var imageRequestModel in roomUpdateModel.Images)
            targetRoom.Images!.Add(new MyImage { Url = await _imageUtility.SaveImageToFile(imageRequestModel,cancellationToken).ConfigureAwait(false)});
        await _roomUnitOfWork.RoomRepository.Update(targetRoom,cancellationToken).ConfigureAwait(false);
        await _roomUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return targetRoom.Adapt<RoomResponseModel>();
    }

}
