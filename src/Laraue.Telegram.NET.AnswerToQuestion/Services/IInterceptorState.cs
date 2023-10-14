namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

/// <summary>
/// Storage to understand the current user context, is interceptor is active now and user request should be redirected to it?
/// </summary>
public interface IInterceptorState<in TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Return <see cref="IRequestInterceptor"/> type if any question should be answered by a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IRequestInterceptor?> TryGetAsync(TKey userId);
    
    /// <summary>
    /// Sets that user should answer the question on the next request.
    /// </summary>
    /// <param name="userId"></param>
    /// <typeparam name="TResponseAwaiter"></typeparam>
    /// <returns></returns>
    Task SetAsync<TResponseAwaiter>(TKey userId) where TResponseAwaiter : IRequestInterceptor;
    
    /// <summary>
    /// Sets that routing for user is executing with standard routing rules.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task ResetAsync(TKey userId);
}