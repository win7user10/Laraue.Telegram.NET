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
    private readonly ITelegramMiddleware _next;
    private readonly IInterceptorState<TKey> _interceptorState;
    private readonly TelegramRequestContext<TKey> _requestContext;
    private readonly IEnumerable<IRequestInterceptor> _interceptors;
    private readonly ILogger<InterceptorsMiddleware<TKey>> _logger;

    public InterceptorsMiddleware(
        ITelegramMiddleware next,
        IInterceptorState<TKey> interceptorState,
        TelegramRequestContext<TKey> requestContext,
        IEnumerable<IRequestInterceptor> interceptors,
        ILogger<InterceptorsMiddleware<TKey>> logger)
    {
        _next = next;
        _interceptorState = interceptorState;
        _requestContext = requestContext;
        _interceptors = interceptors;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task InvokeAsync(CancellationToken ct = default)
    {
        var userId = _requestContext.UserId;
        if (userId is null)
        {
            await _next.InvokeAsync(ct);
            return;
        }
        
        var interceptorId = await _interceptorState.GetAsync(userId);
        if (interceptorId is null)
        {
            await _next.InvokeAsync(ct);
            return;
        }

        var interceptor = _interceptors.FirstOrDefault(x => x.Id == interceptorId);
        if (interceptor is null)
        {
            _logger.LogWarning("Interceptor {Id} has not been found, use default routing mechanism", interceptorId);
            
            await _next.InvokeAsync(ct);
            return;
        }
        
        var result = await interceptor.ExecuteAsync();
        if (result == ExecutionState.FullyExecuted)
        {
            await _interceptorState.ResetAsync(_requestContext.GetUserIdOrThrow());
        }
        
        _requestContext.SetExecutedRoute(new ExecutedRouteInfo("Interceptor", interceptor.ToString()));
    }
}