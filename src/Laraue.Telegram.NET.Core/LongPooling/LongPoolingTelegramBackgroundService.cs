using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Laraue.Telegram.NET.Core.LongPooling;

/// <summary>
/// Service thar receives updates from the tg and run their processing.
/// </summary>
public class LongPoolingTelegramBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IOptions<TelegramNetOptions> _options;
    private readonly ILogger<LongPoolingTelegramBackgroundService> _logger;

    /// <inheritdoc cref="LongPoolingTelegramBackgroundService"/>.
    public LongPoolingTelegramBackgroundService(
        IServiceProvider serviceProvider,
        ITelegramBotClient telegramBotClient,
        IOptions<TelegramNetOptions> options,
        ILogger<LongPoolingTelegramBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _telegramBotClient = telegramBotClient;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Long pooling is not required.
        if (!string.IsNullOrWhiteSpace(_options.Value.WebhookUrl))
        {
            _logger.LogInformation("Telegram webhook url configured, long pooling is disabled");
            
            return;
        }
        
        _logger.LogInformation("Telegram webhook url is not configured, long pooling is enabled");
        
        var currentOffset = 0;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var batchSize = _options.Value.LongPoolingBatchSize
                ?? throw new InvalidOperationException("LongPoolingBatchSize setup is required");

            var longPoolingInterval = _options.Value.LongPoolingInterval
                ?? throw new InvalidOperationException("LongPoolingInterval setup is required");
            
            var updates = await _telegramBotClient
                .GetUpdates(currentOffset, batchSize, cancellationToken: stoppingToken)
                .ConfigureAwait(false);

            if (updates.Length > 0)
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                
                var updatesQueue = scope.ServiceProvider.GetRequiredService<ITelegramUpdatesQueue>();
            
                await updatesQueue.EnqueueAsync(updates, stoppingToken).ConfigureAwait(false);
            
                currentOffset = updates.Last().Id + 1;
            }

            if (updates.Length < batchSize)
            {
                await Task.Delay(longPoolingInterval, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}