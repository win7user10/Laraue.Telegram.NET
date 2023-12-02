using System.Text.Json;

namespace Laraue.Telegram.NET.DataAccess;

/// <summary>
/// Class that allow to build telegram query paths for callback data in the
/// http get method style, like <c>users?age=12</c>.
/// </summary>
public sealed class RoutePathBuilder
{
    private readonly string _routePattern;
    private readonly Lazy<Dictionary<string, string?>> _queryParameters = new (() => new Dictionary<string, string?>());

    /// <summary>
    /// Default route pattern, like <c>users</c>.
    /// </summary>
    /// <param name="routePattern"></param>
    public RoutePathBuilder(string routePattern)
    {
        _routePattern = routePattern;
    }

    /// <summary>
    /// Make a copy of builder, and modify that copy.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public string BuildFor(Action<RoutePathBuilder> action)
    {
        var newBuilder = Copy();

        action(newBuilder);

        return newBuilder.ToString();
    }

    private RoutePathBuilder Copy()
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
    public RoutePathBuilder WithQueryParameter(string parameterName, object? value)
    {
        if (value is null)
        {
            return this;
        }

        _queryParameters.Value[parameterName] = JsonSerializer.SerializeToElement(value).ToString();

        return this;
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
        var result = _routePattern;

        if (!_queryParameters.IsValueCreated)
        {
            return result;
        }
        
        var queryParamsParts = _queryParameters.Value.Keys
            .Select(key => $"{key}={_queryParameters.Value[key]}");

        result = $"{result}?{string.Join('&', queryParamsParts)}";

        return result;
    }
}