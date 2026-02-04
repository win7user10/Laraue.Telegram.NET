using Laraue.Telegram.NET.Abstractions;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Extensions to work with <see cref="TelegramRequestContext"/>.
/// </summary>
public static class TelegramRequestContextExtensions
{
    private const string ExecutedRouteParameter = "ExecutedRoute";

    /// <param name="requestContext"></param>
    extension(TelegramRequestContext requestContext)
    {
        /// <summary>
        /// Returns executed route if it was executed in the request pipeline.
        /// </summary>
        /// <returns></returns>
        public ExecutedRouteInfo? GetExecutedRoute()
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
        /// <param name="route"></param>
        public void SetExecutedRoute(ExecutedRouteInfo route)
        {
            requestContext.Data[ExecutedRouteParameter] = route;
        }
    }
}

/// <summary>
/// Contains information about the executed route.
/// </summary>
/// <param name="Type"></param>
/// <param name="Details"></param>
public sealed record ExecutedRouteInfo(string Type, string? Details);