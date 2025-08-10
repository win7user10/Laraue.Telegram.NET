using Laraue.Telegram.NET.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    /// <inheritdoc cref="LongPoolingTelegramBackgroundService"/>.
    public LongPoolingTelegramBackgroundService(
        IServiceProvider serviceProvider,
        ITelegramBotClient telegramBotClient,
        IOptions<TelegramNetOptions> options)
    {
        _serviceProvider = serviceProvider;
        _telegramBotClient = telegramBotClient;
        _options = options;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Long pooling is not required.
        if (_options.Value.WebhookUrl is not null)
        {
            return;
        }
        
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