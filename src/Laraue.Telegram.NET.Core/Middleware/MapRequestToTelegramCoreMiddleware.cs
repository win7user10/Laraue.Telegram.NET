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
        
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var update = JsonSerializer.Deserialize<Update>(context.Request.Body);
        if (update is null)
        {
            return Task.CompletedTask;
        }

        return _telegramRouter
            .RouteAsync(update, context.RequestAborted);
    }
}