using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

public abstract class BaseQuestionStateStorage : IQuestionStateStorage
{
    private readonly IEnumerable<IAnswerAwaiter> _awaiters;
    private readonly IServiceProvider _serviceProvider;

    protected BaseQuestionStateStorage(IEnumerable<IAnswerAwaiter> awaiters, IServiceProvider serviceProvider)
    {
        _awaiters = awaiters;
        _serviceProvider = serviceProvider;
    }
    
    /// <inheritdoc />
    public async Task<IAnswerAwaiter?> TryGetAsync(string userId)
    {
        var id = await TryGetStringIdentifierFromStorageAsync(userId);

        return _awaiters.FirstOrDefault(x => x.Id == id);
    }

    protected abstract Task<string?> TryGetStringIdentifierFromStorageAsync(string userId);

    /// <inheritdoc />
    public Task SetAsync<TResponseAwaiter>(string userId) where TResponseAwaiter : IAnswerAwaiter
    {
        var awaiter = _serviceProvider.GetRequiredService<TResponseAwaiter>();
        
        return SetStringIdentifierToStorageAsync(userId, awaiter.Id);
    }
    
    protected abstract Task SetStringIdentifierToStorageAsync(string userId, string id);

    /// <inheritdoc />
    public abstract Task ResetAsync(string userId);
}