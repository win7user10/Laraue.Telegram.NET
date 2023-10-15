using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.AnswerToQuestion.Services;
using Laraue.Telegram.NET.Authentication.Middleware;
using Laraue.Telegram.NET.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Laraue.Telegram.NET.AnswerToQuestion.Middleware;

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
    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        var interceptorId = await _interceptorState.GetAsync(_requestContext.UserId!);
        if (interceptorId is null)
        {
            return await _next.InvokeAsync(ct);
        }

        var interceptor = _interceptors.FirstOrDefault(x => x.Id == interceptorId);
        if (interceptor is null)
        {
            _logger.LogWarning("Interceptor {Id} has not been found, use default routing mechanism", interceptorId);
            
            return await _next.InvokeAsync(ct);
        }
        
        var result = await interceptor.ExecuteAsync();
        
        await _interceptorState.ResetAsync(_requestContext.UserId);
        
        return result;
    }
}