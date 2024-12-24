using System.Diagnostics;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Laraue.Telegram.NET.Core.Routing.Middleware;

internal sealed class ExecuteRouteMiddleware(
    IEnumerable<IRoute> routes,
    IServiceProvider serviceProvider,
    TelegramRequestContext requestContext) : ITelegramMiddleware
{

    public async Task InvokeAsync(Func<CancellationToken, Task> next, CancellationToken ct)
    {
        foreach (var route in routes)
        {
            var result = await route.TryExecuteAsync(serviceProvider, ct).ConfigureAwait(false);
            if (!result.IsExecuted)
            {
                continue;
            }

            var routeName = route.ToString();
            requestContext.SetExecutedRoute(new ExecutedRouteInfo("Route", routeName));
            
            await next(ct).ConfigureAwait(false);
            
            return;
        }
        
        await next(ct).ConfigureAwait(false);
    }
}