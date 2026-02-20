using Laraue.Core.Threading;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    IServiceProvider serviceProvider,
    ILogger<TelegramUpdatesService> logger,
    IEnumerable<IExceptionHandler> exceptionHandlers)
    : ITelegramUpdatesService
{
    private readonly KeyedSemaphoreSlim<long> _requestByChatIdSemaphore = new (1);
    
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
        var chatId = update.TryGetChatId();
        chatId ??= 0;
        
        // Different chat messages can be processes in parallel.
        using var requestSemaphore = await _requestByChatIdSemaphore
            .WaitAsync(chatId.Value, cancellationToken)
            .ConfigureAwait(false);
        
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
            
            logger.LogDebug("Processed update {Id}", update.Id);
        }
        catch (Exception e)
        {
            foreach (var handler in exceptionHandlers)
                if (await handler.TryHandleAsync(e, cancellationToken))
                {
                    await updatesQueue
                        .SetProcessedAsync(update, cancellationToken)
                        .ConfigureAwait(false);
                    
                    return;
                }
            
            await updatesQueue
                .SetFailedAsync(
                    update,
                    e.Message,
                    e.StackTrace,
                    cancellationToken)
                .ConfigureAwait(false);
            
            logger.LogError("Error while handling update {Id}, {Message}", update.Id, e.Message);
        }
    }
}