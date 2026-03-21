using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Exceptions;

namespace Laraue.Telegram.NET.Core.Services;

/// <summary>
/// Service thar receives updates from the tg and run their processing.
/// </summary>
public class TelegramUpdatesLongPoolingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<TelegramNetOptions> _options;
    private readonly ILogger<TelegramUpdatesLongPoolingBackgroundService> _logger;

    /// <inheritdoc cref="TelegramUpdatesLongPoolingBackgroundService"/>.
    public TelegramUpdatesLongPoolingBackgroundService(
        IServiceProvider serviceProvider,
        IOptions<TelegramNetOptions> options,
        ILogger<TelegramUpdatesLongPoolingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
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
        
        var batchSize = _options.Value.LongPoolingBatchSize
            ?? throw new InvalidOperationException($"{nameof(TelegramNetOptions.LongPoolingBatchSize)} setup is required");

        var longPoolingInterval = _options.Value.LongPoolingInterval
            ?? throw new InvalidOperationException($"{nameof(TelegramNetOptions.LongPoolingInterval)} setup is required");
        
        var currentOffset = await LoadLastOffsetAsync(stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var telegramUpdatesService = scope.ServiceProvider.GetRequiredService<ITelegramUpdatesService>();

            try
            {
                currentOffset = await telegramUpdatesService
                    .LoadNewUpdatesToQueueAsync(currentOffset, batchSize, stoppingToken)
                    .ConfigureAwait(false);
            }
            catch (RequestException e)
            {
                _logger.LogError(e, "Error while requesting messages from telegram.");
            }
            finally
            {
                await Task
                    .Delay(longPoolingInterval, stoppingToken)
                    .ConfigureAwait(false);
            }
        }
    }

    private async Task<int> LoadLastOffsetAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var updatesQueue = scope.ServiceProvider.GetRequiredService<IUpdatesQueue>();

        return await updatesQueue.GetLastUpdateIdAsync(cancellationToken);
    }
}