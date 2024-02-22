using System.Text;
using System.Text.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace Laraue.Telegram.NET.Core.Routing;

/// <summary>
/// Class that allow to build telegram query paths for callback data in the
/// http query parameters style, like <c>users?name=Alex</c>.
/// </summary>
public sealed class CallbackRoutePath
{
    private readonly string _routePattern;
    private readonly RouteMethod _routeMethod;
    private readonly Lazy<Dictionary<string, object>> _queryParameters = new (() => new Dictionary<string, object>());
    private bool _isFreeze;

    /// <summary>
    /// Default route pattern, like <c>users</c> and the specified method to call it.
    /// </summary>
    public CallbackRoutePath(string routePattern, RouteMethod routeMethod = RouteMethod.Get)
    {
        _routePattern = routePattern;
        _routeMethod = routeMethod;
    }

    /// <summary>
    /// When the builder is freeze each builder modifier returns a new instance of <see cref="CallbackRoutePath"/>. 
    /// </summary>
    public CallbackRoutePath Freeze()
    {
        _isFreeze = true;

        return this;
    }

    private CallbackRoutePath GetCopy()
    {
        var builder = new CallbackRoutePath(_routePattern);

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
    public CallbackRoutePath WithQueryParameter<T>(string parameterName, T? value)
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
    /// Returns callback button for the current <see cref="CallbackRoutePath"/> route.
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
    public static implicit operator string(CallbackRoutePath builder) => builder.ToString();

    /// <summary>
    /// Get the final query string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        
        // Add information about route method to the route
        if (_routeMethod is not RouteMethod.Get)
        {
            sb.Append($"{_routeMethod:D} ");
        }

        sb.Append(_routePattern);
        if (!_queryParameters.IsValueCreated)
        {
            return sb.ToString();
        }

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