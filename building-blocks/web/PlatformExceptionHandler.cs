using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web;

/// <summary>
/// A05/A09: turns unhandled exceptions into a generic RFC 9457 style problem response so stack
/// traces, connection strings and SQL never reach the client, while still recording the full
/// exception in the service log for monitoring and incident response.
/// </summary>
public sealed class PlatformExceptionHandler : IExceptionHandler
{
    private readonly ILogger<PlatformExceptionHandler> _logger;

    public PlatformExceptionHandler(ILogger<PlatformExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception while processing {Method} {Path} for {RemoteIp}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.Connection.RemoteIpAddress);

        if (httpContext.Response.HasStarted) return true;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc9457#section-15.6.1",
            title = "An unexpected error occurred.",
            status = StatusCodes.Status500InternalServerError
        }, cancellationToken);

        return true;
    }
}
