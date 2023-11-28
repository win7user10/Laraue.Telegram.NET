using Laraue.Telegram.NET.Abstractions;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Extensions to work with <see cref="TelegramRequestContext"/>.
/// </summary>
public static class TelegramRequestContextExtensions
{
    private const string ExecutedRouteParameter = "ExecutedRoute";

    /// <summary>
    /// Returns executed route if it was executed in the request pipeline.
    /// </summary>
    /// <param name="requestContext"></param>
    /// <returns></returns>
    public static ExecutedRouteInfo? GetExecutedRoute(this TelegramRequestContext requestContext)
    {
        if (requestContext.Data.TryGetValue(ExecutedRouteParameter, out var localRoute))
        {
            return localRoute as ExecutedRouteInfo;
        }

        return null;
    }
    
    /// <summary>
    /// Sets executed route in the request pipeline.
    /// </summary>
    /// <param name="requestContext"></param>
    /// <param name="route"></param>
    public static void SetExecutedRoute(this TelegramRequestContext requestContext, ExecutedRouteInfo route)
    {
        requestContext.Data[ExecutedRouteParameter] = route;
    }
}

/// <summary>
/// Contains information about the executed route.
/// </summary>
/// <param name="Type"></param>
/// <param name="Details"></param>
public sealed record ExecutedRouteInfo(string Type, string? Details);