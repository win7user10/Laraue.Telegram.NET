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
    private readonly string _pathPattern;

    /// <summary>
    /// Telegram update type.
    /// </summary>
    public UpdateType UpdateType { get; }
    
    /// <summary>
    /// Pattern of path which will be tried to match.
    /// </summary>
    public Regex PathPattern { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TelegramBaseRouteWithPathAttribute"/>.
    /// </summary>
    /// <param name="updateType"></param>
    /// <param name="pathPattern"></param>
    protected TelegramBaseRouteWithPathAttribute(UpdateType updateType, string pathPattern)
    {
        _pathPattern = pathPattern;
        UpdateType = updateType;
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

        var pathFromUpdate = GetPathFromUpdate(update);
        if (pathFromUpdate is null)
        {
            return false;
        }
        
        var match = PathPattern.Match(pathFromUpdate);
        if (!match.Success)
        {
            return false;
        }
        
        var queryParameters = pathFromUpdate.ParseQueryParts();
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
    protected abstract string? GetPathFromUpdate(Update update);
}