using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.Interceptors.Services;

public abstract class BaseInterceptorState<TKey> : IInterceptorState<TKey>
     where TKey : IEquatable<TKey>
{
    private readonly IServiceProvider _serviceProvider;

    protected BaseInterceptorState(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public abstract Task<string?> GetAsync(TKey userId);

    /// <inheritdoc />
    public Task<TContext?> GetInterceptorContextAsync<TContext>(TKey userId)
        where TContext : class
    {
        return typeof(TContext) == typeof(EmptyContext)
            ? (Task.FromResult(EmptyContext.Value) as Task<TContext?>)!
            : GetInterceptorContextInternalAsync<TContext>(userId);
    }

    /// <summary>
    /// Get interceptor context from the storage.
    /// </summary>
    /// <param name="userId"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    protected abstract Task<TContext?> GetInterceptorContextInternalAsync<TContext>(TKey userId)
        where TContext : class;

    /// <inheritdoc />
    public Task SetAsync<TInterceptor, TInterceptorContext>(TKey userId, TInterceptorContext data)
        where TInterceptor : IRequestInterceptor<TInterceptorContext>
        where TInterceptorContext : class
    {
        var interceptor = _serviceProvider.GetRequiredService<TInterceptor>();

        return SetAsync(interceptor, userId, data);
    }

    /// <inheritdoc />
    public async Task SetAsync<TInterceptorContext>(
        IRequestInterceptor<TInterceptorContext> interceptor,
        TKey userId,
        TInterceptorContext data) where TInterceptorContext : class
    {
        await interceptor.BeforeInterceptorSetAsync(data).ConfigureAwait(false);
        
        await SetInterceptorAsync(userId, interceptor.Id, data).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task SetAsync<TInterceptor>(TKey userId) where TInterceptor : IRequestInterceptor<EmptyContext>
    {
        return SetAsync<TInterceptor, EmptyContext>(userId, EmptyContext.Value);
    }

    /// <inheritdoc />
    public Task SetAsync(IRequestInterceptor<EmptyContext> interceptor, TKey userId)
    {
        return SetAsync(interceptor, userId, EmptyContext.Value);
    }

    /// <summary>
    /// Describes how to save a string identifier of the current question to the storage.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected abstract Task SetInterceptorAsync<TContext>(TKey userId, string id, TContext context);

    /// <inheritdoc />
    public abstract Task ResetAsync(TKey userId);
}