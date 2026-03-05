using Laraue.Telegram.NET.Core.Routing;
using Laraue.Telegram.NET.Core.Utils;
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
            "Request time {Time} ms, status: no endpoint to execute for request type: {Type}, payload: {Payload}",
            routeNotFoundException.ElapsedMilliseconds,
            routeNotFoundException.Payload.Type,
            TelegramUpdateSerializer.Serialize(routeNotFoundException.Payload));

        return Task.FromResult(true);
    }
}