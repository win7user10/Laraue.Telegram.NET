using Laraue.Telegram.NET.Abstractions.Exceptions;
using Laraue.Telegram.NET.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

/// <summary>
/// The common case of interceptor is to
/// 1. Take a request from a user
/// 2. Validate the passed data
/// 3. Show message that data is not valid, if validation is not passed
/// 4. Process the request if data is valid. 
/// </summary>
/// <typeparam name="TUserKey">Key type of the user in the system.</typeparam>
/// <typeparam name="TModel">Type of data that required to execute the request.</typeparam>
public abstract class BaseRequestInterceptor<TUserKey, TModel> : IRequestInterceptor
    where TUserKey : IEquatable<TUserKey>
{
    /// <inheritdoc />
    public abstract string Id { get; }

    /// <inheritdoc />
    public async Task<object?> ExecuteAsync(IServiceProvider serviceProvider)
    {
        var telegramRequestContext = serviceProvider
            .GetRequiredService<TelegramRequestContext<TUserKey>>();
        
        var answerResult = new InterceptResult<TModel>();

        await ValidateAsync(telegramRequestContext, answerResult).ConfigureAwait(false);
        if (answerResult.Error is not null)
        {
            throw new BadTelegramRequestException(answerResult.Error);
        }
        
        if (answerResult.Model is null)
        {
            throw new InvalidOperationException("Model should be bind if validation finished successfully.");
        }

        return await ExecuteRouteAsync(telegramRequestContext, answerResult.Model).ConfigureAwait(false);
    }

    /// <summary>
    /// In this method validation should be passed.
    /// If any error occured, <see cref="InterceptResult{TResult}"/> should had an error.
    /// If errors are not occured <see cref="InterceptResult{TResult}.Model"/> in the results should be bind.
    /// </summary>
    /// <param name="requestContext">Current telegram request context.</param>
    /// <param name="interceptResult">Validation result.</param>
    protected abstract Task ValidateAsync(
        TelegramRequestContext<TUserKey> requestContext,
        InterceptResult<TModel> interceptResult);
    
    /// <summary>
    /// Execute the awaiter body if validation has been passed successfully.
    /// </summary>
    /// <param name="requestContext">Current telegram request context.</param>
    /// <param name="model">Validated model.</param>
    /// <returns></returns>
    protected abstract Task<object?> ExecuteRouteAsync(
        TelegramRequestContext<TUserKey> requestContext,
        TModel model);
}