using System.Text;
using System.Text.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace Laraue.Telegram.NET.Core.Routing;

/// <summary>
/// Class that allow to build telegram query paths for callback data in the
/// http get method style, like <c>users?age=12</c>.
/// </summary>
public sealed class RoutePathBuilder
{
    private readonly string _routePattern;
    private readonly Lazy<Dictionary<string, object>> _queryParameters = new (() => new Dictionary<string, object>());
    private bool _isFreeze;

    /// <summary>
    /// Default route pattern, like <c>users</c>.
    /// </summary>
    /// <param name="routePattern"></param>
    public RoutePathBuilder(string routePattern)
    {
        _routePattern = routePattern;
    }

    /// <summary>
    /// When the builder is freeze each builder modifier returns a new instance of <see cref="RoutePathBuilder"/>. 
    /// </summary>
    public RoutePathBuilder Freeze()
    {
        _isFreeze = true;

        return this;
    }

    private RoutePathBuilder GetCopy()
    {
        var builder = new RoutePathBuilder(_routePattern);

        if (!_queryParameters.IsValueCreated)
        {
            return builder;
        }
        
        foreach (var (key, value) in _queryParameters.Value)
        {
            builder._queryParameters.Value[key] = value;
        }

        return builder;
    }

    /// <summary>
    /// Add query parameter to the path.
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public RoutePathBuilder WithQueryParameter<T>(string parameterName, T? value)
    {
        var builder = _isFreeze ? GetCopy() : this;
        
        if (value is null)
        {
            return builder;
        }

        builder._queryParameters.Value[parameterName] = value;

        return builder;
    }

    /// <summary>
    /// Returns callback button for the current <see cref="RoutePathBuilder"/> route.
    /// </summary>
    public InlineKeyboardButton ToInlineKeyboardButton(string text)
    {
        return InlineKeyboardButton.WithCallbackData(text, ToString());
    }

    /// <summary>
    /// Get the final route string from the builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static implicit operator string(RoutePathBuilder builder) => builder.ToString();

    /// <summary>
    /// Get the final query string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (!_queryParameters.IsValueCreated)
        {
            return _routePattern;
        }

        var sb = new StringBuilder(_routePattern);

        sb.Append('?');
        
        foreach (var queryParameter in _queryParameters.Value.Keys)
        {
            var value = _queryParameters.Value[queryParameter];
            var jsonValue = JsonSerializer.Serialize(value, Defaults.JsonOptions);
            
            sb.Append(queryParameter)
                .Append('=')
                .Append(jsonValue)
                .Append('&');
        }

        sb.Remove(sb.Length - 1, 1);

        return sb.ToString();
    }
}