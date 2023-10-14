using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.AnswerToQuestion.Services;
using Laraue.Telegram.NET.Authentication.Middleware;
using Laraue.Telegram.NET.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

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
    private readonly IServiceProvider _serviceProvider;

    public InterceptorsMiddleware(
        ITelegramMiddleware next,
        IInterceptorState<TKey> interceptorState,
        TelegramRequestContext<TKey> requestContext,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _interceptorState = interceptorState;
        _requestContext = requestContext;
        _serviceProvider = serviceProvider;
    }
    
    /// <inheritdoc />
    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        var responseAwaiter = await _interceptorState.TryGetAsync(_requestContext.UserId!);

        if (responseAwaiter is null)
        {
            return await _next.InvokeAsync(ct);
        }
        
        var result = await responseAwaiter.ExecuteAsync(_serviceProvider);
        
        await _interceptorState.ResetAsync(_requestContext.UserId);
        
        return result;
    }
}