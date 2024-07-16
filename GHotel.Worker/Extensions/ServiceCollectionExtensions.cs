using System.Globalization;
using System.Net;
using GHotel.Application.Extensions;
using GHotel.Infrastructure.Configuration;
using GHotel.Infrastructure.Extensions;
using GHotel.Worker.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace GHotel.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddApplicationServices(configuration);

        services.Configure<EmailConfiguration>(configuration.GetSection("Logging").GetSection("Email"));

        using var scope = services.BuildServiceProvider().CreateScope();
        var emailConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<EmailConfiguration>>().Value;
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Email(
                from: emailConfiguration.From,
                to: emailConfiguration.To,
                port: int.Parse(emailConfiguration.Port, CultureInfo.CurrentCulture),
                host: emailConfiguration.Host,
                credentials: new NetworkCredential(emailConfiguration.Username, emailConfiguration.Password),
                formatProvider: CultureInfo.CurrentCulture,
                restrictedToMinimumLevel: LogEventLevel.Fatal
            )
            .CreateLogger();

        services.AddHostedService<ReservationWorker>();

        return services;
    }
}
