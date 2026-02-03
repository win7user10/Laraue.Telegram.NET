using Laraue.Core.Threading;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Services;

public class TelegramQueueBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<TelegramNetOptions> _options;
    private static readonly KeyedSemaphoreSlim<long> RequestSemaphore = new (1);

    public TelegramQueueBackgroundService(
        IServiceProvider serviceProvider,
        IOptions<TelegramNetOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var batchSize = _options.Value.TelegramUpdatesInMemoryQueueMaxCount;

        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var updatesQueue = scope.ServiceProvider.GetRequiredService<IUpdatesQueue>();
        
            var updates = await updatesQueue
                .GetAsync(
                    batchSize,
                    stoppingToken)
                .ConfigureAwait(false);
            
            var tasks = updates
                .Select(update => ProcessUpdateAsync(update, stoppingToken));
        
            await Task.WhenAll(tasks).ConfigureAwait(false);

            if (updates.Length == 0)
            {
                await Task
                    .Delay(_options.Value.TelegramUpdatesPoolInterval, stoppingToken)
                    .ConfigureAwait(false);
            }
        }
    }

    private async Task ProcessUpdateAsync(
        Update update,
        CancellationToken cancellationToken)
    {
        // One user updates should be processed sequentially
        if (update.TryGetUserId(out var userId))
        {
            await RequestSemaphore
                .WaitAsync(userId.Value, cancellationToken)
                .ConfigureAwait(false);
        }
        
        await using var scope = _serviceProvider.CreateAsyncScope();
        
        var telegramRouter = scope.ServiceProvider.GetRequiredService<ITelegramRouter>();
        var updatesQueue = scope.ServiceProvider.GetRequiredService<IUpdatesQueue>();

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
    }
}