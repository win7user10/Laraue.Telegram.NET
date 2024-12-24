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
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly TelegramRequestContext _telegramRequestContext;

    /// <summary>
    /// Initializes a new instance of <see cref="HandleExceptionsMiddleware"/>.
    /// </summary>
    /// <param name="telegramBotClient"></param>
    /// <param name="telegramRequestContext"></param>
    public HandleExceptionsMiddleware(
        ITelegramBotClient telegramBotClient,
        TelegramRequestContext telegramRequestContext)
    {
        _telegramBotClient = telegramBotClient;
        _telegramRequestContext = telegramRequestContext;
    }

    /// <inheritdoc />
    public async Task InvokeAsync(Func<CancellationToken, Task> next, CancellationToken ct)
    {
        try
        {
            await next(ct).ConfigureAwait(false);
        }
        catch (BadTelegramRequestException ex)
        {
            var userId = _telegramRequestContext.Update.GetUserId();
            
            await _telegramBotClient
                .SendMessage(
                    userId,
                    ex.Message,
                    cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}