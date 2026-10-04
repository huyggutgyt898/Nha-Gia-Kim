using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nha_Gia_Kim.Services;

namespace Nha_Gia_Kim.Middleware;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ResourceNotFoundException => (StatusCodes.Status404NotFound, "Not Found", exception.Message),
            BusinessConflictException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
            InvalidOperationRequestException => (StatusCodes.Status400BadRequest, "Invalid Request", exception.Message),
            _ => (0, string.Empty, string.Empty)
        };

        if (statusCode == 0)
        {
            return false;
        }

        logger.LogWarning(exception, "Handled API request error: {Title}", title);
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}
