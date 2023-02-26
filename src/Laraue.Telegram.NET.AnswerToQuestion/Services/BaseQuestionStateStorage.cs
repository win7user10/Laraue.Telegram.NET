using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

public abstract class BaseQuestionStateStorage<TKey> : IQuestionStateStorage<TKey>
     where TKey : IEquatable<TKey>
{
    private readonly IEnumerable<IAnswerAwaiter> _awaiters;
    private readonly IServiceProvider _serviceProvider;

    protected BaseQuestionStateStorage(
        IEnumerable<IAnswerAwaiter> awaiters,
        IServiceProvider serviceProvider)
    {
        _awaiters = awaiters;
        _serviceProvider = serviceProvider;
    }
    
    /// <inheritdoc />
    public async Task<IAnswerAwaiter?> TryGetAsync(TKey userId)
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
    public Task SetAsync<TResponseAwaiter>(TKey userId) where TResponseAwaiter : IAnswerAwaiter
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