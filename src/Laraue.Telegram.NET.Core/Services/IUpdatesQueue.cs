using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Services;

public interface IUpdatesQueue
{
    Task AddAsync(
        IEnumerable<Update> updates,
        CancellationToken cancellationToken = default);
    
    Task SetProcessedAsync(
        Update update,
        CancellationToken cancellationToken = default);
    
    Task SetFailedAsync(
        Update update,
        string error,
        string? stackTrace,
        CancellationToken cancellationToken = default);

    Task<Update[]> GetAsync(
        int count,
        CancellationToken cancellationToken = default);
    
    Task<int> GetLastUpdateIdAsync(CancellationToken cancellationToken = default);
}