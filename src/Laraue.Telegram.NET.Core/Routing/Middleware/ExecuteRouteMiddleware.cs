using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;

namespace Laraue.Telegram.NET.Core.Routing.Middleware;

internal sealed class ExecuteRouteMiddleware : ITelegramMiddleware
{
    private readonly IEnumerable<IRoute> _routes;
    private readonly IServiceProvider _serviceProvider;
    private readonly TelegramRequestContext _requestContext;

    public ExecuteRouteMiddleware(IEnumerable<IRoute> routes, IServiceProvider serviceProvider, TelegramRequestContext requestContext)
    {
        _routes = routes;
        _serviceProvider = serviceProvider;
        _requestContext = requestContext;
    }

    public async Task InvokeAsync(Func<CancellationToken, Task> next, CancellationToken ct)
    {
        foreach (var route in _routes)
        {
            var result = await route.TryExecuteAsync(_serviceProvider, ct).ConfigureAwait(false);
            
            if (!result.IsExecuted)
            {
                continue;
            }

            _requestContext.SetExecutedRoute(new ExecutedRouteInfo("Route", route.ToString()));
            
            await next(ct).ConfigureAwait(false);
            return;
        }
        
        await next(ct).ConfigureAwait(false);
    }
}