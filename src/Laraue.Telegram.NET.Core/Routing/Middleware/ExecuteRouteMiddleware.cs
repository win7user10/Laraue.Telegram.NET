using System.Diagnostics;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Laraue.Telegram.NET.Core.Routing.Middleware;

internal sealed class ExecuteRouteMiddleware(
    IEnumerable<IRoute> routes,
    IServiceProvider serviceProvider,
    TelegramRequestContext requestContext,
    ILogger<ExecuteRouteMiddleware> logger) : ITelegramMiddleware
{

    public async Task InvokeAsync(Func<CancellationToken, Task> next, CancellationToken ct)
    {
        var sw = new Stopwatch();
        sw.Start();
        
        foreach (var route in routes)
        {
            var result = await route.TryExecuteAsync(serviceProvider, ct).ConfigureAwait(false);
            
            if (!result.IsExecuted)
            {
                continue;
            }

            var routeName = route.ToString();
            requestContext.SetExecutedRoute(new ExecutedRouteInfo("Route", routeName));
            logger.LogDebug("Route {Name} has been matched for {Time} ms", routeName, sw.ElapsedMilliseconds);
            
            sw.Restart();
            await next(ct).ConfigureAwait(false);
            logger.LogDebug("Route {Name} has been executed for {Time} ms", routeName, sw.ElapsedMilliseconds);
            
            return;
        }
        
        await next(ct).ConfigureAwait(false);
    }
}