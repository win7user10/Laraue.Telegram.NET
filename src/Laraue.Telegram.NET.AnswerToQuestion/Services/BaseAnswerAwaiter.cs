using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Abstractions.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

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

        Validate(telegramRequestContext.Update, answerResult);
        if (answerResult.Error is not null)
        {
            throw new BadTelegramRequestException(answerResult.Error);
        }
        
        if (answerResult.Model is null)
        {
            throw new InvalidOperationException("Model should be bind if validation finished successfully.");
        }

        return await ExecuteRouteAsync(answerResult.Model);
    }

    /// <summary>
    /// In this method validation should be passed.
    /// If any error occured, <see cref="AnswerResult{TResult}"/> should has an error.
    /// If errors are not occured <see cref="AnswerResult{TResult}.Model"/> in the results should be bind.
    /// </summary>
    /// <param name="update"></param>
    /// <param name="answerResult"></param>
    protected abstract void Validate(Update update, AnswerResult<TModel> answerResult);
    
    /// <summary>
    /// Execute the awaiter body if validation has been passed successfully.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    protected abstract Task<object?> ExecuteRouteAsync(TModel model);
}