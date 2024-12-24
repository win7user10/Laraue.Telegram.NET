using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Laraue.Telegram.NET.Core.Middleware;

/// <summary>
/// Automatically response on telegram callback calls if the request finished successfully.
/// </summary>
public sealed class AutoCallbackResponseMiddleware : ITelegramMiddleware
{
    private readonly TelegramRequestContext _requestContext;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ILogger<AutoCallbackResponseMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="AutoCallbackResponseMiddleware"/>.
    /// </summary>
    public AutoCallbackResponseMiddleware(
        TelegramRequestContext requestContext,
        ITelegramBotClient telegramBotClient,
        ILogger<AutoCallbackResponseMiddleware> logger)
    {
        _requestContext = requestContext;
        _telegramBotClient = telegramBotClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task InvokeAsync(Func<CancellationToken, Task> next, CancellationToken ct = default)
    {
        var callbackQueryId = _requestContext.Update.CallbackQuery?.Id;
        return callbackQueryId is null ? next(ct) : InvokeInternalAsync(next, callbackQueryId, ct);
    }

    private async Task InvokeInternalAsync(Func<CancellationToken, Task> next, string callbackQueryId, CancellationToken ct)
    {
        await next(ct).ConfigureAwait(false);

        if (_requestContext.GetExecutedRoute() is null)
        {
            return;
        }
        
        try
        {
            _logger.LogDebug("Responding to the callback query {Id}", callbackQueryId);
                
            await _telegramBotClient
                .AnswerCallbackQuery(
                    callbackQueryId,
                    cancellationToken: ct)
                .ConfigureAwait(false);
        }
        catch (ApiRequestException e)
        {
            _logger.LogWarning(e, "Failed responding to the callback query {Id}", callbackQueryId);
        }
    }
}