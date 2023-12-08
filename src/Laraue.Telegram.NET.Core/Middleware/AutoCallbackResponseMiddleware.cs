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
    private readonly ITelegramMiddleware _next;
    private readonly TelegramRequestContext _requestContext;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ILogger<AutoCallbackResponseMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="AutoCallbackResponseMiddleware"/>.
    /// </summary>
    public AutoCallbackResponseMiddleware(
        ITelegramMiddleware next,
        TelegramRequestContext requestContext,
        ITelegramBotClient telegramBotClient,
        ILogger<AutoCallbackResponseMiddleware> logger)
    {
        _next = next;
        _requestContext = requestContext;
        _telegramBotClient = telegramBotClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task InvokeAsync(CancellationToken ct = default)
    {
        var callbackQueryId = _requestContext.Update.CallbackQuery?.Id;
        return callbackQueryId is null ? _next.InvokeAsync(ct) : InvokeInternalAsync(callbackQueryId, ct);
    }

    private async Task InvokeInternalAsync(string callbackQueryId, CancellationToken ct)
    {
        await _next.InvokeAsync(ct);

        if (_requestContext.GetExecutedRoute() is null)
        {
            return;
        }
        
        try
        {
            _logger.LogDebug("Responding to the callback query {Id}", callbackQueryId);
                
            await _telegramBotClient.AnswerCallbackQueryAsync(
                callbackQueryId,
                cancellationToken: ct);
        }
        catch (ApiRequestException e)
        {
            _logger.LogWarning(e, "Failed responding to the callback query {Id}", callbackQueryId);
        }
    }
}