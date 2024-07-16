using GHotel.API.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace GHotel.API.Infrastructure.Middlewares;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LocalizationConfiguration _localizationConfiguration;

    public LocalizationMiddleware(RequestDelegate next, IOptions<LocalizationConfiguration> localizationConfiguration)
    {
        _next = next;
        _localizationConfiguration = localizationConfiguration.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var culutureName = _localizationConfiguration.DefaultCulture;
        var queryCulture = context.Request.Headers["Accept-Language"].ToString();
        if (!string.IsNullOrEmpty(queryCulture))
        {
            var receivedCulutureName = queryCulture.Split(',')[0];
            if (_localizationConfiguration.SupportedCultures.Contains(receivedCulutureName))
                culutureName = receivedCulutureName;
        }

        var culture = CultureInfo.GetCultureInfo(culutureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        await _next(context).ConfigureAwait(false);
    }
}
