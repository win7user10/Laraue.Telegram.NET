using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Authentication.Services;
using Laraue.Telegram.NET.Core.Extensions;
using Laraue.Telegram.NET.Interceptors.Services;
using Microsoft.Extensions.Logging;

namespace Laraue.Telegram.NET.Interceptors.Middleware;

/// <summary>
/// Middleware to support interceptors functionality.
/// </summary>
public class InterceptorsMiddleware<TKey> : ITelegramMiddleware
    where TKey : IEquatable<TKey>
{
    private readonly IInterceptorState<TKey> _interceptorState;
    private readonly TelegramRequestContext<TKey> _requestContext;
    private readonly IEnumerable<IRequestInterceptor> _interceptors;
    private readonly ILogger<InterceptorsMiddleware<TKey>> _logger;

    public InterceptorsMiddleware(
        IInterceptorState<TKey> interceptorState,
        TelegramRequestContext<TKey> requestContext,
        IEnumerable<IRequestInterceptor> interceptors,
        ILogger<InterceptorsMiddleware<TKey>> logger)
    {
        _interceptorState = interceptorState;
        _requestContext = requestContext;
        _interceptors = interceptors;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task InvokeAsync(Func<CancellationToken, Task> next, CancellationToken ct = default)
    {
        var userId = _requestContext.UserId;
        if (userId is null)
        {
            await next(ct);
            return;
        }
        
        var interceptorId = await _interceptorState.GetAsync(userId).ConfigureAwait(false);
        if (interceptorId is null)
        {
            await next(ct);
            return;
        }

        var interceptor = _interceptors.FirstOrDefault(x => x.Id == interceptorId);
        if (interceptor is null)
        {
            _logger.LogWarning("Interceptor {Id} has not been found, use default routing mechanism", interceptorId);
            
            await next(ct);
            return;
        }
        
        var result = await interceptor.ExecuteAsync().ConfigureAwait(false);
        if (result is ExecutionState.FullyExecuted or ExecutionState.Cancelled)
        {
            await _interceptorState.ResetAsync(_requestContext.GetUserIdOrThrow()).ConfigureAwait(false);
        }
        
        _requestContext.SetExecutedRoute(new ExecutedRouteInfo("Interceptor", interceptor.ToString()));
    }
}