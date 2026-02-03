using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Services;

public interface IUpdatesQueue
{
    Task AddAsync(
        IEnumerable<Update> updates,
        CancellationToken cancellationToken);
    
    Task SetProcessedAsync(
        Update update,
        CancellationToken cancellationToken);
    
    Task SetFailedAsync(
        Update update,
        string error,
        string? stackTrace,
        CancellationToken cancellationToken);

    Task<Update[]> GetAsync(
        int count,
        CancellationToken cancellationToken);
}