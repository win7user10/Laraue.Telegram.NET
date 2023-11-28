using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;
using Telegram.Bot;

namespace Laraue.Telegram.NET.Core.Middleware;

/// <summary>
/// Automatically response on telegram callback calls if the request finished successfully.
/// </summary>
public sealed class AutoCallbackResponseMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly TelegramRequestContext _requestContext;
    private readonly ITelegramBotClient _telegramBotClient;

    /// <summary>
    /// Initializes a new instance of <see cref="AutoCallbackResponseMiddleware"/>.
    /// </summary>
    public AutoCallbackResponseMiddleware(
        ITelegramMiddleware next,
        TelegramRequestContext requestContext,
        ITelegramBotClient telegramBotClient)
    {
        _next = next;
        _requestContext = requestContext;
        _telegramBotClient = telegramBotClient;
    }

    /// <inheritdoc />
    public Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        var callbackQueryId = _requestContext.Update.CallbackQuery?.Id;
        return callbackQueryId is null ? _next.InvokeAsync(ct) : InvokeInternalAsync(callbackQueryId, ct);
    }

    private async Task<object?> InvokeInternalAsync(string callbackQueryId, CancellationToken ct)
    {
        var result = await _next.InvokeAsync(ct);

        if (_requestContext.GetExecutedRoute() is not null)
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(
                callbackQueryId,
                cancellationToken: ct);
        }

        return result;
    }
}