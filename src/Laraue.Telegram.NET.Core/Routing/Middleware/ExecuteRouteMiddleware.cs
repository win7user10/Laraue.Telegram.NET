using Laraue.Telegram.NET.Abstractions;

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

    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        foreach (var route in _routes)
        {
            var result = await route.TryExecuteAsync(_serviceProvider);
            
            if (!result.IsExecuted)
            {
                continue;
            }

            _requestContext.ExecutedRoute = route;
            
            return result.ExecutionResult;
        }

        return null;
    }
}