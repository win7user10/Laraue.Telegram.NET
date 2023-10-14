using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

public abstract class BaseInterceptorState<TKey> : IInterceptorState<TKey>
     where TKey : IEquatable<TKey>
{
    private readonly IEnumerable<IRequestInterceptor> _awaiters;
    private readonly IServiceProvider _serviceProvider;

    protected BaseInterceptorState(
        IEnumerable<IRequestInterceptor> awaiters,
        IServiceProvider serviceProvider)
    {
        _awaiters = awaiters;
        _serviceProvider = serviceProvider;
    }
    
    /// <inheritdoc />
    public async Task<IRequestInterceptor?> TryGetAsync(TKey userId)
    {
        var id = await TryGetStringIdentifierFromStorageAsync(userId);

        return _awaiters.FirstOrDefault(x => x.Id == id);
    }

    /// <summary>
    /// Describes how to retrieve a string identifier of the current question to the storage.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    protected abstract Task<string?> TryGetStringIdentifierFromStorageAsync(TKey userId);

    /// <inheritdoc />
    public Task SetAsync<TResponseAwaiter>(TKey userId) where TResponseAwaiter : IRequestInterceptor
    {
        var awaiter = _serviceProvider.GetRequiredService<TResponseAwaiter>();
        
        return SetStringIdentifierToStorageAsync(userId, awaiter.Id);
    }
    
    /// <summary>
    /// Describes how to save a string identifier of the current question to the storage.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    protected abstract Task SetStringIdentifierToStorageAsync(TKey userId, string id);

    /// <inheritdoc />
    public abstract Task ResetAsync(TKey userId);
}