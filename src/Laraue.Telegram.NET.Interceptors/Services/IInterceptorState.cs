namespace Laraue.Telegram.NET.Interceptors.Services;

/// <summary>
/// Storage to understand the current user context, is interceptor is active now and user request should be redirected to it?
/// </summary>
public interface IInterceptorState<in TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Return identifier of the interceptor if it is set for the user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<string?> GetAsync(TKey userId);
    
    /// <summary>
    /// Return interceptor context for the current interceptor if it is exist.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<TContext?> GetInterceptorContextAsync<TContext>(TKey userId)
        where TContext : class;
    
    /// <summary>
    /// Sets that user should answer the question on the next request
    /// and sets context data for that.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <typeparam name="TInterceptor"></typeparam>
    /// <typeparam name="TInterceptorContext"></typeparam>
    /// <returns></returns>
    Task SetAsync<TInterceptor, TInterceptorContext>(TKey userId, TInterceptorContext data)
        where TInterceptor : IRequestInterceptor<TInterceptorContext>
        where TInterceptorContext : class;
    
    /// <summary>
    /// Sets that user should answer the question on the next request.
    /// </summary>
    /// <param name="userId"></param>
    /// <typeparam name="TInterceptor"></typeparam>
    /// <returns></returns>
    Task SetAsync<TInterceptor>(TKey userId) where TInterceptor : IRequestInterceptor<EmptyContext>;
    
    /// <summary>
    /// Sets that routing for user is executing with standard routing rules.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task ResetAsync(TKey userId);
}