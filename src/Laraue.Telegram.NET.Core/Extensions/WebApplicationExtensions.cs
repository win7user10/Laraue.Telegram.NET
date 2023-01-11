using Laraue.Telegram.NET.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Extensions to map requests from telegram to the Telegram.NET library.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Sets telegram requests root path. This url will be handled via Telegram.NET library.
    /// </summary>
    /// <param name="applicationBuilder"></param>
    /// <param name="route"></param>
    public static void MapTelegramRequests(this IApplicationBuilder applicationBuilder, string route)
    {
        applicationBuilder.MapWhen(
            x => x.Request.Path == route,
            builder => builder.UseMiddleware<MapRequestToTelegramCoreMiddleware>());
    }

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

            var routeResult = await _telegramRouter.RouteAsync(update);

            if (routeResult is string stringResult)
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(stringResult);
            }
            
            else if (routeResult is not null)
            {
                context.Response.ContentType = "text/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(routeResult));
            }
        }
    }
}