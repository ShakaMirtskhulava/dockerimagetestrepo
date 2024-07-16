using System.Globalization;
using GHotel.Application.Repositories;
using GHotel.Application.UOW;
using GHotel.Application.Utilities;
using GHotel.Infrastructure.Configuration;
using GHotel.Infrastructure.Repositories;
using GHotel.Infrastructure.UOW;
using GHotel.Infrastructure.Utilities;
using GHotel.Infrastructure.Utilities.Payment;
using GHotel.Persistance;
using GHotel.Persistance.Configurations.OptionsConfigurations;
using GHotel.Persistance.Context;
using GHotel.Persistance.Interceptors;
using HashidsNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GHotel.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<ConnectionStrings>(configuration.GetSection(nameof(ConnectionStrings)));
        services.Configure<AppConfiguration>(configuration.GetSection(nameof(AppConfiguration)));
        services.Configure<PayPalConfigurationSettings>(configuration.GetSection("PayMent:PayPal"));
        services.Configure<AdminCredentials>(configuration.GetSection(nameof(AdminCredentials)));
        services.Configure<TBCCurrencyConvertionSettings>(configuration.GetSection(nameof(TBCCurrencyConvertionSettings)));

        services.AddScoped<UpdateEntitiesInterceptor>();
        services.AddDbContext<GHotelDBContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();
        services.AddScoped<IRoomUnitOfWork, RoomUnitOfWork>();
        services.AddScoped<IImageUnitOfWork, ImageUnitOfWork>();
        services.AddScoped<IReservationUnitOfWork, ReservationUnitOfWork>();
        services.AddScoped<IPaymentUnitOfWork, PaymentUnitOfWork>();

        services.AddSingleton<IHashids>(_ => new Hashids(configuration["HashidsConfiguration:Salt"],
            int.Parse(configuration["HashidsConfiguration:MinHashLength"],CultureInfo.CurrentCulture)));

        services.AddScoped<IImageUtility, ImageUtility>();
        services.AddScoped<IPaymentUtility, PayPalPaymentUtility>();
        services.AddScoped<ICurrencyConvertionUtility, TBCCurrencyConvertion>();
        services.AddScoped<IIdHasherUtility, HashIdsUtility>();
        services.AddSingleton<ILockUtility, SemaphorLockUtility>();
        services.AddScoped<IPasswordHasherUtility, PasswordHasherUtility>();
        services.AddScoped<IEmailSenderUtility, EmailSenderUtility>();
    }
}
