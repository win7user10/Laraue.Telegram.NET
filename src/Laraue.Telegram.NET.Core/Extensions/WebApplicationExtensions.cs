using Laraue.Telegram.NET.Core.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Extensions to map requests from telegram to the Telegram.NET library.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Sets telegram requests root path. This url will be handled via Telegram.NET library.
    /// Required for webhook mode only.
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
    /// Sets telegram requests root path. The url will be loaded from the <see cref="TelegramNetOptions"/>.
    /// </summary>
    /// <param name="applicationBuilder"></param>
    public static void MapTelegramRequests(this IApplicationBuilder applicationBuilder)
    {
        var options = applicationBuilder.ApplicationServices
            .GetRequiredService<IOptions<TelegramNetOptions>>().Value;

        if (options.WebhookUrl == null)
        {
            throw new InvalidOperationException("WebhookUrl is not configured");
        }
        
        applicationBuilder.MapTelegramRequests(options.WebhookUrl);
    }
}