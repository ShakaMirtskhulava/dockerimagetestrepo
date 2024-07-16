using System.Globalization;
using System.Net;
using GHotel.API.Infrastructure.Middlewares;
using GHotel.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace GHotel.API.Infrastructure.Extensions;

public static class LoggingExtensions
{
    public static void ConfigureLogging(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Logging.ClearProviders();
        webApplicationBuilder.Logging.AddSerilog();

        using var scope = webApplicationBuilder.Services.BuildServiceProvider().CreateScope();
        EmailConfiguration emailConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<EmailConfiguration>>().Value;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(webApplicationBuilder.Configuration)
            .WriteTo.Email(
                from: emailConfiguration.From,
                to: emailConfiguration.To,
                port: int.Parse(emailConfiguration.Port,CultureInfo.CurrentCulture),
                host: emailConfiguration.Host,
                credentials: new NetworkCredential(emailConfiguration.Username, emailConfiguration.Password),
                formatProvider: CultureInfo.CurrentCulture,
                restrictedToMinimumLevel: LogEventLevel.Fatal
            )
            .CreateLogger();
    }

    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }

}
