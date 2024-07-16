using GHotel.API.Infrastructure.Extensions;
using GHotel.API.Infrastructure.Mapping;
using GHotel.Application.Extensions;
using GHotel.Infrastructure.Extensions;
using GHotel.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureLocalization(builder.Configuration);
builder.Services.ConfigureOptions(builder.Configuration);

builder.ConfigureLogging(); 

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.ConfigureFluentValidations();
builder.Services.ConfigureMapping();

builder.Services.ConfigureAuthentication();
builder.Services.ConfigureAuthorization();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Cache");
});

var app = builder.Build();

app
   .UseLocalization()
   .UseCORS(builder.Configuration)
   .UseGlobalExceptionHandling()
   .UseRequestResponseLogging();

app.UseSwaggerApiVersioning();
if(app.Environment.IsProduction())
    app.UseHsts();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await Seed.Initialize(app.Services).ConfigureAwait(false);

app.Run();
