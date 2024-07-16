using System.Net;
using GHotel.API.Infrastructure.Localization;
using GHotel.Application.Exceptions;
using GHotel.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GHotel.API.Infrastructure.Error;

public class APIError : ProblemDetails
{
    private const string UnhandledErrorCode = "UnhandledErrorCode";
    private readonly HttpContext _httpContext;
    private readonly Exception _exception;

    public string Code { get; set; }
    public string? TraceId
    {
        get
        {
            if (Extensions.TryGetValue("TraceId", out var traceId))
                return (string?)traceId;
            return null;
        }
        set => Extensions["TraceId"] = value;
    }

    public APIError(Exception exception, HttpContext httpContext)
    {
        _httpContext = httpContext;
        _exception = exception;
        Status = (int)HttpStatusCode.InternalServerError;
        Code = UnhandledErrorCode;
        TraceId = httpContext.TraceIdentifier;
        Title = ErrorMessages.GlobalExceptionTitle;
        Instance = httpContext.Request.Path;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";

        HandleException((dynamic)exception);
    }

    public void HandleException(Exception exception)
    {
    }

    public void HandleException(InvalidPasswordException exception)
    {
        Status = (int)HttpStatusCode.BadRequest;
        Code = exception.Code;
        Title = ErrorMessages.InvalidPasswordExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
    }
    public void HandleException(MyUnauthorizedException exception)
    {
        Status = (int)HttpStatusCode.Unauthorized;
        Code = exception.Code;
        Title = ErrorMessages.MyUnauthorizedExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
    }
    public void HandleException(UnauthorizedAccessException exception)
    {
        Status = (int)HttpStatusCode.Unauthorized;
        Code = "Unauthorized";
        Title = ErrorMessages.UnauthorizedAccessExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
    }

    public void HandleException(NotFoundException exception)
    {
        Status = (int)HttpStatusCode.NotFound;
        Code = exception.Code;
        Title = ErrorMessages.NotFoundExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
    }

    public void HandleException(RoomAlreadyBookedException exception)
    {
        Status = (int)HttpStatusCode.BadRequest;
        Code = exception.Code;
        Title = ErrorMessages.RoomAlreadyBookedExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
    }
    public void HandleException(PaymentNotCreatedException exception)
    {
        Status = (int)HttpStatusCode.InternalServerError;
        Code = exception.Code;
        Title = ErrorMessages.PaymentNotCreatedExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
    }
    public void HandleException(OperationFailedException exception)
    {
        Status = (int)HttpStatusCode.BadRequest;
        Code = exception.Code;
        Title = ErrorMessages.OperationFailedExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
    }
    public void HandleException(UserNotFoundException exception)
    {
        Status = (int)HttpStatusCode.NotFound;
        Code = exception.Code;
        Title = ErrorMessages.UserNotFoundExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
    }
    public void HandleException(UserWithEmailNotFoundException exception)
    {
        Status = (int)HttpStatusCode.NotFound;
        Code = exception.Code;
        Title = ErrorMessages.UserWithEmailNotFoundExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
    }
    public void HandleException(ImageFileNotFoundException exception)
    {
        Status = (int)HttpStatusCode.InternalServerError;
        Code = exception.Code;
        Title = ErrorMessages.ImageFileNotFoundExceptionTitle;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
    }

}
