﻿namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

/// <summary>
/// Storage to understand the current user context, is any question is active now and user should answer it?
/// </summary>
public interface IQuestionStateStorage
{
    /// <summary>
    /// Return <see cref="IAnswerAwaiter"/> type if any question should be answered by a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Type?> TryGetAsync(string userId);
    
    /// <summary>
    /// Sets that user should answer the question on the next request.
    /// </summary>
    /// <param name="userId"></param>
    /// <typeparam name="TResponseAwaiter"></typeparam>
    /// <returns></returns>
    Task SetAsync<TResponseAwaiter>(string userId) where TResponseAwaiter : IAnswerAwaiter;
    
    /// <summary>
    /// Sets that routing for user is executing with standard routing rules.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task ResetAsync(string userId);
}