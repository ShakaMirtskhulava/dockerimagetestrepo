using GHotel.API.Infrastructure.Middlewares;

namespace GHotel.API.Infrastructure.Extensions;

public static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder appBuilder)
    {
        appBuilder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        return appBuilder;
    }
}
