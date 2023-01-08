using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Telegram.NET.Core.Routing.Attributes;

/// <summary>
/// Attribute that describe matching of telegram request with controller method
/// based on the <see cref="UpdateType"/> of <see cref="Update"/> request and the
/// path of the request. 
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public abstract class TelegramBaseRouteWithPathAttribute : TelegramBaseRouteAttribute
{
    /// <summary>
    /// Telegram update type.
    /// </summary>
    public UpdateType UpdateType { get; }
    
    /// <summary>
    /// Pattern of path which will be tried to match.
    /// </summary>
    public Regex PathPattern { get; }

    protected TelegramBaseRouteWithPathAttribute(UpdateType updateType, string pathPattern)
    {
        UpdateType = updateType;
        PathPattern = RouteRegexCreator.ForRoute(pathPattern);
    }

    public override bool IsMatch(Update update)
    {
        if (update.Type != UpdateType)
        {
            return false;
        }

        var pathFromUpdate = GetPathFromUpdate(update);
        return pathFromUpdate is not null && PathPattern.IsMatch(pathFromUpdate);
    }

    /// <summary>
    /// Returns path identifier from the <see cref="Update"/>.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    protected abstract string? GetPathFromUpdate(Update update);
}