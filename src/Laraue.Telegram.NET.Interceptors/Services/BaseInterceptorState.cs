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
    public abstract Task<string?> GetAsync(
        TKey userId,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public Task<TContext?> GetInterceptorContextAsync<TContext>(
        TKey userId,
        CancellationToken cancellationToken = default)
        where TContext : class
    {
        return typeof(TContext) == typeof(EmptyContext)
            ? (Task.FromResult(EmptyContext.Value) as Task<TContext?>)!
            : GetInterceptorContextInternalAsync<TContext>(userId);
    }

    /// <summary>
    /// Get interceptor context from the storage.
    /// </summary>
    /// <returns></returns>
    protected abstract Task<TContext?> GetInterceptorContextInternalAsync<TContext>(
        TKey userId,
        CancellationToken cancellationToken = default)
        where TContext : class;

    /// <inheritdoc />
    public Task SetAsync<TInterceptor, TInterceptorContext>(
        TKey userId,
        TInterceptorContext data,
        CancellationToken cancellationToken = default)
        where TInterceptor : IRequestInterceptor<TInterceptorContext>
        where TInterceptorContext : class
    {
        var interceptor = _serviceProvider.GetRequiredService<TInterceptor>();

        return SetAsync(interceptor, userId, data, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetAsync<TInterceptorContext>(
        IRequestInterceptor<TInterceptorContext> interceptor,
        TKey userId,
        TInterceptorContext data,
        CancellationToken cancellationToken = default)
        where TInterceptorContext : class
    {
        await interceptor.BeforeInterceptorSetAsync(data, cancellationToken).ConfigureAwait(false);
        
        await SetInterceptorAsync(userId, interceptor.Id, data, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task SetAsync<TInterceptor>(
        TKey userId,
        CancellationToken cancellationToken = default)
        where TInterceptor : IRequestInterceptor<EmptyContext>
    {
        return SetAsync<TInterceptor, EmptyContext>(userId, EmptyContext.Value, cancellationToken);
    }

    /// <inheritdoc />
    public Task SetAsync(
        IRequestInterceptor<EmptyContext> interceptor,
        TKey userId,
        CancellationToken cancellationToken = default)
    {
        return SetAsync(interceptor, userId, EmptyContext.Value, cancellationToken);
    }

    /// <summary>
    /// Describes how to save a string identifier of the current question to the storage.
    /// </summary>
    /// <returns></returns>
    protected abstract Task SetInterceptorAsync<TContext>(
        TKey userId,
        string id,
        TContext context,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task ResetAsync(
        TKey userId,
        CancellationToken cancellationToken = default);
}