using GHotel.Application.Services.ImageService;
using GHotel.Application.Services.PaymentService;
using GHotel.Application.Services.ReservationService;
using GHotel.Application.Services.RoomService;
using GHotel.Application.Services.UserService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GHotel.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IPaymentService, PaymentService>();
    }
}
