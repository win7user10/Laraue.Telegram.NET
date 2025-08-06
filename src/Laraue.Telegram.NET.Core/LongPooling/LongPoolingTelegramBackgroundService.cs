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
    private readonly LongPoolingOptions _options;

    /// <inheritdoc cref="LongPoolingTelegramBackgroundService"/>.
    public LongPoolingTelegramBackgroundService(
        IServiceProvider serviceProvider,
        ITelegramBotClient telegramBotClient,
        LongPoolingOptions options)
    {
        _serviceProvider = serviceProvider;
        _telegramBotClient = telegramBotClient;
        _options = options;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentOffset = 0;

        var maxIntervalTimeDifference = _options.MaxIntervalBetweenUpdatesCheckMs - _options.MinIntervalBetweenUpdatesCheckMs;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var requestsWithoutUpdates = 0;
            
            var updates = await _telegramBotClient
                .GetUpdates(currentOffset, _options.BatchSize, cancellationToken: stoppingToken)
                .ConfigureAwait(false);
            
            foreach (var update in updates)
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                
                var telegramRouter = scope.ServiceProvider.GetRequiredService<ITelegramRouter>();

                await telegramRouter.RouteAsync(update, stoppingToken).ConfigureAwait(false);

                currentOffset = update.Id + 1;
            }

            if (!updates.Any())
            {
                requestsWithoutUpdates++;
            }

            if (updates.Length < _options.BatchSize)
            {
                var minWaitTime = 
                
                // TODO - dynamic value
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}