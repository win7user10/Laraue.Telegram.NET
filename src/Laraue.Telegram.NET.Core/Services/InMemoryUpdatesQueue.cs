using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Services;

public class InMemoryUpdatesQueue : IUpdatesQueue
{
    private readonly List<Update> _updates = [];
    
    public Task AddAsync(IEnumerable<Update> updates, CancellationToken cancellationToken)
    {
        _updates.AddRange(updates);
        
        return Task.CompletedTask;
    }

    public Task SetProcessedAsync(Update update, CancellationToken cancellationToken)
    {
        _updates.Remove(update);
        
        return Task.CompletedTask;
    }

    public Task SetFailedAsync(
        Update update,
        string error,
        string? stackTrace,
        CancellationToken cancellationToken = default)
    {
        return SetProcessedAsync(update, cancellationToken);
    }

    public Task<Update[]> GetAsync(int count, CancellationToken cancellationToken)
    {
        return Task.FromResult(_updates.Take(count).ToArray());
    }

    public Task<int> GetLastUpdateIdAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_updates.Select(u => u.Id).DefaultIfEmpty().Max());
    }
}