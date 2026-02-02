using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.LongPooling;

public interface ITelegramUpdatesQueue
{
    /// <summary>
    /// Enqueue taken telegram updates to the application queue. 
    /// </summary>
    Task EnqueueAsync(
        IEnumerable<Update> updates,
        CancellationToken cancellationToken);

    /// <summary>
    /// Returns the latest update for each registered user (limited by <paramref name="batchSize" />).
    /// </summary>
    Task<IEnumerable<Update>> PopAsync(
        int batchSize,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Remove processed update from the queue.
    /// </summary>
    Task RemoveAsync(
        long updateId,
        CancellationToken cancellationToken);
}

public class TelegramUpdatesQueue : ITelegramUpdatesQueue
{
    public Task EnqueueAsync(IEnumerable<Update> updates, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Update>> PopAsync(int batchSize, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(long updateId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}