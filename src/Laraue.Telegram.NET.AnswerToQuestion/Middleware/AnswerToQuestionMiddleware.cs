using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.AnswerToQuestion.Services;
using Laraue.Telegram.NET.Authentication.Middleware;
using Laraue.Telegram.NET.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.AnswerToQuestion.Middleware;

/// <summary>
/// Middleware to support answer to the question functionality.
/// </summary>
public class AnswerToQuestionMiddleware<TKey> : ITelegramMiddleware
    where TKey : IEquatable<TKey>
{
    private readonly ITelegramMiddleware _next;
    private readonly IQuestionStateStorage<TKey> _questionStateStorage;
    private readonly TelegramRequestContext<TKey> _requestContext;
    private readonly IServiceProvider _serviceProvider;

    public AnswerToQuestionMiddleware(
        ITelegramMiddleware next,
        IQuestionStateStorage<TKey> questionStateStorage,
        TelegramRequestContext<TKey> requestContext,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _questionStateStorage = questionStateStorage;
        _requestContext = requestContext;
        _serviceProvider = serviceProvider;
    }
    
    /// <inheritdoc />
    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        var responseAwaiter = await _questionStateStorage.TryGetAsync(_requestContext.UserId!);

        if (responseAwaiter is null)
        {
            return await _next.InvokeAsync(ct);
        }
        
        var result = await responseAwaiter.ExecuteAsync(_serviceProvider);
        
        await _questionStateStorage.ResetAsync(_requestContext.UserId);
        
        return result;
    }
}