using GHotel.API.Infrastructure.Error;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using System.Net;
using System.Text.Json;

namespace GHotel.API.Infrastructure.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = requestDelegate;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex).ConfigureAwait(false);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        APIError apiError = new(exception, httpContext);
        var errorJson = JsonSerializer.Serialize(apiError);

        if(apiError.Status == (int)HttpStatusCode.InternalServerError)
            _logger.LogCritical(errorJson + $"\nException Message:{exception.Message} \nInnerException: {exception.InnerException?.Message} \nStack Trace: {exception.StackTrace}");
        else
            _logger.LogError(errorJson + $"\nException Message:{exception.Message} \nInnerException: {exception.InnerException?.Message} \nStack Trace: {exception.StackTrace}");

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = apiError.Status!.Value;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(errorJson).ConfigureAwait(false);
    }

}
