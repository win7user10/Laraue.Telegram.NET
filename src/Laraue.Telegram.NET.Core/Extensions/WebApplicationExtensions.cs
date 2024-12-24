using Laraue.Telegram.NET.Core.Middleware;
using Laraue.Telegram.NET.Core.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

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
    
    /// <summary>
    /// Use webhooks for telegram requests handling.
    /// </summary>
    /// <param name="applicationBuilder"></param>
    public static void MapLongPoolingRequests(this IApplicationBuilder applicationBuilder)
    {
        var s = applicationBuilder.ApplicationServices.GetRequiredService<ILongPoolingRequestsProcessor>();
        var b = applicationBuilder.ApplicationServices.GetRequiredService<ITelegramBotClient>();

        b.OnApiResponseReceived += (_, args, token) => s.ProcessAsync(args.ResponseMessage, token);
    }
}