using GHotel.Worker.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

try
{
    var builder = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((hostContext, services) =>
        {
            services.ConfigureServices(configuration);
        })
        .Build();

    await builder.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred while running the worker.");
    Console.WriteLine(ex.Message);
}
