using Laraue.Telegram.NET.Core.Routing;
using Microsoft.Extensions.Logging;

namespace Laraue.Telegram.NET.Core.Services;

public class RouteNotFoundExceptionHandler(ILogger<RouteNotFoundExceptionHandler> logger)
    : IExceptionHandler
{
    public Task<bool> TryHandleAsync(
        Exception exception,
        CancellationToken cancellationToken = default)
    {
        if (exception is not RouteNotFoundException routeNotFoundException)
        {
            return Task.FromResult(false);
        }
        
        logger.LogInformation(
            "Request time {Time} ms, status: no endpoint to execute, payload: {Payload}",
            routeNotFoundException.ElapsedMilliseconds,
            routeNotFoundException.Payload);

        return Task.FromResult(true);
    }
}