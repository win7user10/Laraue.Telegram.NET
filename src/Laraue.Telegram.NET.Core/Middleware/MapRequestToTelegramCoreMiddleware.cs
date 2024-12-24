using System.Text.Json;
using Laraue.Telegram.NET.Abstractions;
using Microsoft.AspNetCore.Http;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Middleware;

internal sealed class MapRequestToTelegramCoreMiddleware : IMiddleware
{
    private readonly ITelegramRouter _telegramRouter;

    public MapRequestToTelegramCoreMiddleware(ITelegramRouter telegramRouter)
    {
        _telegramRouter = telegramRouter;
    }
        
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var update = await JsonSerializer.DeserializeAsync<Update>(context.Request.Body);
        if (update is null)
        {
            return;
        }

        await _telegramRouter
            .RouteAsync(update, context.RequestAborted)
            .ConfigureAwait(false);
    }
}