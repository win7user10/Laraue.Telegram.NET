using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Abstractions.Exceptions;
using Laraue.Telegram.NET.Core.Extensions;
using Telegram.Bot;

namespace Laraue.Telegram.NET.Core.Routing.Middleware;

public class HandleExceptionsMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly TelegramRequestContext _telegramRequestContext;

    public HandleExceptionsMiddleware(
        ITelegramMiddleware next,
        ITelegramBotClient telegramBotClient,
        TelegramRequestContext telegramRequestContext)
    {
        _next = next;
        _telegramBotClient = telegramBotClient;
        _telegramRequestContext = telegramRequestContext;
    }
    
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