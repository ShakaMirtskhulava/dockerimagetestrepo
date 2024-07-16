using GHotel.Application.Services.ReservationService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GHotel.Worker.Workers;

public class ReservationWorker : BackgroundService
{
    private readonly ILogger<ReservationWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ReservationWorker(ILogger<ReservationWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await reservationService.CompleteReservations(stoppingToken).ConfigureAwait(false);
                await Task.Delay(10000, stoppingToken).ConfigureAwait(false);
            }
            catch(TaskCanceledException)
            {
                _logger.LogInformation("The worker was cancelled.");
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while running the worker.");
            }
        }
    }
}
