using System.Diagnostics;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;
using Laraue.Telegram.NET.Core.Routing.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Routing;

/// <inheritdoc />
public sealed class TelegramRouter : ITelegramRouter
{
    private readonly MiddlewareList _middlewareList;
    private readonly IServiceProvider _serviceProvider;
    private readonly TelegramRequestContext _telegramRequestContext;
    private readonly ILogger<TelegramRouter> _logger;

    
    /// <summary>
    /// Initializes a new instance of <see cref="TelegramRouter"/>.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="telegramRequestContext"></param>
    /// <param name="middlewareList"></param>
    /// <param name="logger"></param>
    public TelegramRouter(
        IServiceProvider serviceProvider,
        TelegramRequestContext telegramRequestContext,
        IOptions<MiddlewareList> middlewareList,
        ILogger<TelegramRouter> logger)
    {
        _middlewareList = middlewareList.Value;
        _serviceProvider = serviceProvider;
        _telegramRequestContext = telegramRequestContext;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task RouteAsync(Update update, CancellationToken cancellationToken = default)
    {
        var sw = new Stopwatch();
        sw.Start();

        _telegramRequestContext.Update = update;
        
        ITelegramMiddleware? lastMiddleware = null;
        foreach (var middlewareType in _middlewareList.Items)
        {
            var middleware = lastMiddleware == null
                ? ActivatorUtilities.CreateInstance(_serviceProvider, middlewareType)
                : ActivatorUtilities.CreateInstance(_serviceProvider, middlewareType, lastMiddleware);

            lastMiddleware = (ITelegramMiddleware) middleware;
        }

        await lastMiddleware!.InvokeAsync(cancellationToken);
        var executedRoute = _telegramRequestContext.GetExecutedRoute();
        
        if (executedRoute is not null)
        {
            _logger.LogInformation(
                "Request time {Time} ms, route: {RouteName} executed",
                sw.ElapsedMilliseconds,
                executedRoute);
        }
        else
        {
            _logger.LogInformation(
                "Request time {Time} ms, status: not found, payload: {Payload}",
                sw.ElapsedMilliseconds,
                JsonConvert.SerializeObject(update));
        }
    }
}