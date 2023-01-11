using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Abstractions.Exceptions;
using Laraue.Telegram.NET.Core.Extensions;
using Telegram.Bot;

namespace Laraue.Telegram.NET.Core.Routing.Middleware;

/// <summary>
/// The standard logic of exceptions handling for the telegram requests.
/// Bad requests are used to send info to the user about wrong passed data.
/// Other exceptions are throws.
/// </summary>
public class HandleExceptionsMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly TelegramRequestContext _telegramRequestContext;

    /// <summary>
    /// Initializes a new instance of <see cref="HandleExceptionsMiddleware"/>.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="telegramBotClient"></param>
    /// <param name="telegramRequestContext"></param>
    public HandleExceptionsMiddleware(
        ITelegramMiddleware next,
        ITelegramBotClient telegramBotClient,
        TelegramRequestContext telegramRequestContext)
    {
        _next = next;
        _telegramBotClient = telegramBotClient;
        _telegramRequestContext = telegramRequestContext;
    }

    /// <inheritdoc />
    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        try
        {
            return await _next.InvokeAsync(ct);
        }
        catch (BadTelegramRequestException ex)
        {
            var userId = _telegramRequestContext.Update.GetUserId();
            
            await _telegramBotClient.SendTextMessageAsync(
                userId,
                ex.Message,
                cancellationToken: ct);
        }

        return null;
    }
}