using Laraue.Telegram.NET.Abstractions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
        using var sr = new StreamReader(context.Request.Body);
        var body = await sr.ReadToEndAsync();
            
        var update = JsonConvert.DeserializeObject<Update>(body);
        if (update is null)
        {
            return;
        }

        await _telegramRouter.RouteAsync(update, context.RequestAborted);
    }
}