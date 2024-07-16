using System.Diagnostics;

namespace GHotel.API.Infrastructure.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate requestDelegate, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = requestDelegate;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        _logger.LogInformation($"Request: {httpContext.Request.Method} {httpContext.Request.Path} {httpContext.Request.QueryString}");
        _logger.LogInformation($"User IP: {httpContext.Connection.RemoteIpAddress}");
        _logger.LogInformation($"Request received at: {DateTime.UtcNow}");
        if (httpContext.Request.Method == "POST" || httpContext.Request.Method == "PUT")
        {
            httpContext.Request.EnableBuffering();
            if (httpContext.Request.ContentType == null || !httpContext.Request.ContentType.Contains("multipart/form-data"))
            {
                var body = await new StreamReader(httpContext.Request.Body).ReadToEndAsync().ConfigureAwait(false);
                httpContext.Request.Body.Position = 0;
                _logger.LogInformation($"Request body: {body}");
            }
        }

        var sw = new Stopwatch();
        sw.Start();

        await _next(httpContext).ConfigureAwait(false);

        sw.Stop();
        _logger.LogInformation($"Response: {httpContext.Response.StatusCode} in {sw.ElapsedMilliseconds}ms");
        _logger.LogInformation($"Response sent at: {DateTime.UtcNow}");
    }

}
