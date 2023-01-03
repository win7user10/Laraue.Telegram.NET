using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.AnswerToQuestion.Services;
using Laraue.Telegram.NET.Authentication.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.AnswerToQuestion.Middleware;

public class AnswerToQuestionMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly IQuestionStateStorage _questionStateStorage;
    private readonly TelegramRequestContext _requestContext;
    private readonly IServiceProvider _serviceProvider;

    public AnswerToQuestionMiddleware(
        ITelegramMiddleware next,
        IQuestionStateStorage questionStateStorage,
        TelegramRequestContext requestContext,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _questionStateStorage = questionStateStorage;
        _requestContext = requestContext;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        if (_requestContext.UserId is null)
        {
            throw new InvalidOperationException(
                $"User information is not available." +
                $" Ensure {typeof(AuthTelegramMiddleware)} has been registered");
        }
        
        var responseAwaiterType = await _questionStateStorage.TryGetAsync(_requestContext.UserId!);

        if (responseAwaiterType is null)
        {
            return await _next.InvokeAsync(ct);
        }
        
        var responseAwaiter = (_serviceProvider.GetRequiredService(responseAwaiterType) as IAnswerAwaiter)!;
        var result = await responseAwaiter.ExecuteAsync(_serviceProvider);
        
        await _questionStateStorage.ResetAsync(_requestContext.UserId);
        
        return result;
    }
}