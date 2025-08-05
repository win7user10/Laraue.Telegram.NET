using Laraue.Telegram.NET.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Laraue.Telegram.NET.Core.LongPooling;

/// <summary>
/// Sercie thar receives updates from the tg and run their processing.
/// </summary>
public class LongPoolingTelegramBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITelegramBotClient _telegramBotClient;

    /// <inheritdoc cref="LongPoolingTelegramBackgroundService"/>.
    public LongPoolingTelegramBackgroundService(
        IServiceProvider serviceProvider,
        ITelegramBotClient telegramBotClient)
    {
        _serviceProvider = serviceProvider;
        _telegramBotClient = telegramBotClient;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentOffset = 0;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            const int batchSize = 10;
            
            var updates = await _telegramBotClient
                .GetUpdates(currentOffset, batchSize, cancellationToken: stoppingToken)
                .ConfigureAwait(false);
            
            foreach (var update in updates)
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                
                var telegramRouter = scope.ServiceProvider.GetRequiredService<ITelegramRouter>();

                await telegramRouter.RouteAsync(update, stoppingToken).ConfigureAwait(false);

                currentOffset = update.Id + 1;
            }

            if (updates.Length < batchSize)
            {
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}