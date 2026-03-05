using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Services;

public interface IUpdatesQueue
{
    /// <summary>
    /// Store updates to the queue.
    /// </summary>
    Task AddAsync(
        IEnumerable<Update> updates,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Set the update state as successfully processed.
    /// </summary>
    Task SetProcessedAsync(
        Update update,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Set the update state as failed.
    /// </summary>
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