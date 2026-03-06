namespace Laraue.Telegram.NET.Interceptors.Services;

/// <summary>
/// Storage to understand the current user context, is interceptor is active now and user request should be redirected to it?
/// </summary>
public interface IInterceptorState<in TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Return identifier of the interceptor if it is set for the user.
    /// </summary>
    /// <returns></returns>
    Task<string?> GetAsync(TKey userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Return interceptor context for the current interceptor if it is exist.
    /// </summary>
    /// <returns></returns>
    Task<TContext?> GetInterceptorContextAsync<TContext>(TKey userId, CancellationToken cancellationToken = default)
        where TContext : class;
    
    /// <summary>
    /// Sets that user should answer the question on the next request and sets context data for that.
    /// </summary>
    /// <returns></returns>
    Task SetAsync<TInterceptor, TInterceptorContext>(
        TKey userId,
        TInterceptorContext data,
        CancellationToken cancellationToken = default)
        where TInterceptor : IRequestInterceptor<TInterceptorContext>
        where TInterceptorContext : class;
    
    /// <summary>
    /// Sets that user should answer the question on the next request and sets context data for that.
    /// </summary>
    /// <returns></returns>
    Task SetAsync<TInterceptorContext>(
        IRequestInterceptor<TInterceptorContext> interceptor,
        TKey userId,
        TInterceptorContext data,
        CancellationToken cancellationToken = default)
        where TInterceptorContext : class;
    
    /// <summary>
    /// Sets that user should answer the question on the next request.
    /// </summary>
    /// <returns></returns>
    Task SetAsync<TInterceptor>(TKey userId, CancellationToken cancellationToken = default)
        where TInterceptor : IRequestInterceptor<EmptyContext>;
    
    /// <summary>
    /// Sets that user should answer the question on the next request.
    /// </summary>
    /// <returns></returns>
    Task SetAsync(
        IRequestInterceptor<EmptyContext> interceptor,
        TKey userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets that routing for user is executing with standard routing rules.
    /// </summary>
    /// <returns></returns>
    Task ResetAsync(TKey userId, CancellationToken cancellationToken = default);
}