using Laraue.Telegram.NET.Core.Middleware;
using Microsoft.AspNetCore.Builder;

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
}