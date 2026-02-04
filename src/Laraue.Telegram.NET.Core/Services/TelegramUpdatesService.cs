using Laraue.Core.Threading;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Services;

public interface ITelegramUpdatesService
{
    /// <summary>
    /// Load all new updates to the application <see cref="IUpdatesQueue"/>.
    /// Returns latest update offset.
    /// </summary>
    Task<int> LoadNewUpdatesToQueueAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Process all queue updates.
    /// </summary>
    /// <param name="batchSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ProcessQueueUpdatesAsync(
        int batchSize,
        CancellationToken cancellationToken = default);
}

public class TelegramUpdatesService(
    ITelegramBotClient telegramBotClient,
    IUpdatesQueue updatesQueue,
    IServiceProvider serviceProvider)
    : ITelegramUpdatesService
{
    private static readonly KeyedSemaphoreSlim<long> RequestSemaphore = new (1);
    
    public async Task<int> LoadNewUpdatesToQueueAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var updates = await telegramBotClient
                .GetUpdates(offset, limit, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (updates.Length > 0)
            {
                await updatesQueue.AddAsync(updates, cancellationToken).ConfigureAwait(false);
            
                offset = updates.Last().Id + 1;
            }
            
            if (updates.Length < limit)
            {
                return offset;
            }
        }

        return offset;
    }

    public async Task ProcessQueueUpdatesAsync(
        int batchSize,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var updates = await updatesQueue
                .GetAsync(
                    batchSize,
                    cancellationToken)
                .ConfigureAwait(false);

            if (updates.Length > 0)
            {
                var tasks = updates
                    .Select(update => ProcessUpdateAsync(update, cancellationToken));
        
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            else
            {
                break;
            }
        }
    }
    
    private async Task ProcessUpdateAsync(
        Update update,
        CancellationToken cancellationToken)
    {
        IDisposable? userSemaphore = null;
        
        // One user updates should be processed sequentially
        if (update.TryGetUserId(out var userId))
        {
            userSemaphore = await RequestSemaphore
                .WaitAsync(userId.Value, cancellationToken)
                .ConfigureAwait(false);
        }
        
        await using var scope = serviceProvider.CreateAsyncScope();
        var telegramRouter = scope.ServiceProvider.GetRequiredService<ITelegramRouter>();

        try
        {
            await telegramRouter
                .RouteAsync(update, cancellationToken)
                .ConfigureAwait(false);

            await updatesQueue
                .SetProcessedAsync(update, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await updatesQueue
                .SetFailedAsync(
                    update,
                    e.Message,
                    e.StackTrace,
                    cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            userSemaphore?.Dispose();
        }
    }
}