using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Laraue.Telegram.NET.Core.Services;

public class TelegramQueueBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<TelegramNetOptions> _options;

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
        var waitInterval = _options.Value.TelegramUpdatesPoolInterval;

        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var updatesService = scope.ServiceProvider.GetRequiredService<ITelegramUpdatesService>();
            
            await updatesService.ProcessQueueUpdatesAsync(batchSize, stoppingToken);
            
            await Task
                .Delay(waitInterval, stoppingToken)
                .ConfigureAwait(false);
        }
    }
}