using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Abstractions.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

/// <summary>
/// The common case of awaiter is to
/// 1. Take a request from a user
/// 2. Validate the passed data
/// 3. Show message that data is not valid, if validation is not passed
/// 4. Process the request if data is valid. 
/// </summary>
/// <typeparam name="TModel">Type of data that required to execute the request.</typeparam>
public abstract class BaseAnswerAwaiter<TModel> : IAnswerAwaiter
{
    public async Task<object?> ExecuteAsync(IServiceProvider serviceProvider)
    {
        var telegramRequestContext = serviceProvider.GetRequiredService<TelegramRequestContext>();
        var answerResult = new AnswerResult<TModel>();

        Validate(telegramRequestContext, answerResult);
        if (answerResult.Error is not null)
        {
            throw new BadTelegramRequestException(answerResult.Error);
        }
        
        if (answerResult.Model is null)
        {
            throw new InvalidOperationException("Model should be bind if validation finished successfully.");
        }

        return await ExecuteRouteAsync(telegramRequestContext, answerResult.Model);
    }

    /// <summary>
    /// In this method validation should be passed.
    /// If any error occured, <see cref="AnswerResult{TResult}"/> should has an error.
    /// If errors are not occured <see cref="AnswerResult{TResult}.Model"/> in the results should be bind.
    /// </summary>
    /// <param name="requestContext">Current telegram request context.</param>
    /// <param name="answerResult">Validation result.</param>
    protected abstract void Validate(TelegramRequestContext requestContext, AnswerResult<TModel> answerResult);
    
    /// <summary>
    /// Execute the awaiter body if validation has been passed successfully.
    /// </summary>
    /// <param name="requestContext">Current telegram request context.</param>
    /// <param name="model">Validated model.</param>
    /// <returns></returns>
    protected abstract Task<object?> ExecuteRouteAsync(TelegramRequestContext requestContext, TModel model);
}