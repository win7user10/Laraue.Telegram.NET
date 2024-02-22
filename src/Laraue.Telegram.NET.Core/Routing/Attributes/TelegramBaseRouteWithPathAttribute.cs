using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Laraue.Telegram.NET.Abstractions.Request;
using Laraue.Telegram.NET.Core.Extensions;
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
    /// Controller route method type.
    /// </summary>
    public RouteMethod RouteMethod { get; }
    
    /// <summary>
    /// Pattern of path which will be tried to match.
    /// </summary>
    public Regex PathPattern { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TelegramBaseRouteWithPathAttribute"/>.
    /// </summary>
    /// <param name="updateType"></param>
    /// <param name="routeMethod"></param>
    /// <param name="pathPattern"></param>
    protected TelegramBaseRouteWithPathAttribute(UpdateType updateType, RouteMethod routeMethod, string pathPattern)
    {
        UpdateType = updateType;
        RouteMethod = routeMethod;
        PathPattern = RouteRegexCreator.ForRoute(pathPattern);
    }

    /// <inheritdoc />
    public override bool TryMatch(Update update, [NotNullWhen(true)] out RequestParameters? requestParameters)
    {
        requestParameters = null;
        
        if (update.Type != UpdateType)
        {
            return false;
        }

        var dataString = GetDataStringFromUpdate(update);
        if (dataString is null)
        {
            return false;
        }
        
        var routeMethod = TakeMethodBlock(dataString, out var pathString);
        if (routeMethod != RouteMethod)
        {
            return false;
        }
        
        var match = PathPattern.Match(pathString);
        if (!match.Success)
        {
            return false;
        }
        
        var queryParameters = pathString.ParseQueryParts();
        var pathParameters = match.Groups
            .Cast<Group>()
            .Where(x => !int.TryParse(x.Name, out _))
            .ToDictionary(x => x.Name, x => x.Value);

        requestParameters = new RequestParameters(pathParameters, queryParameters);
        return true;
    }

    /// <summary>
    /// Returns path identifier from the <see cref="Update"/>.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    protected abstract string? GetDataStringFromUpdate(Update update);

    private static RouteMethod TakeMethodBlock(string dataString, out string pathString)
    {
        if (dataString[1] == ' ')
        {
            pathString = dataString[2..];
            return Enum.Parse<RouteMethod>(dataString.AsSpan(0, 1));
        }

        pathString = dataString;
        return RouteMethod.Get;
    }
}