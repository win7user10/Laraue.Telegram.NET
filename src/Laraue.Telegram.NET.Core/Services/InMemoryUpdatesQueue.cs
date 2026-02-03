using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Services;

public class InMemoryUpdatesQueue(ILogger<InMemoryUpdatesQueue> logger)
    : IUpdatesQueue
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

    public Task SetFailedAsync(Update update, string error, string? stackTrace, CancellationToken cancellationToken)
    {
        logger.LogError($"{error} {stackTrace}");
        
        SetProcessedAsync(update, cancellationToken);

        return Task.CompletedTask;
    }

    public Task<Update[]> GetAsync(int count, CancellationToken cancellationToken)
    {
        return Task.FromResult(_updates.Take(count).ToArray());
    }
}