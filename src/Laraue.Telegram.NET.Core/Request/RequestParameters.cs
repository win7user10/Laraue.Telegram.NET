using System.Text.Json;

namespace Laraue.Telegram.NET.Core.Request;

/// <summary>
/// Contains parameters dictionary where each parameter can
/// be retrieved and deserialized into the necessary type. 
/// </summary>
public class RequestParameters : Dictionary<string, string?>
{
    /// <inheritdoc />
    public RequestParameters()
        : base(StringComparer.OrdinalIgnoreCase)
    { 
    }
    
    /// <summary>
    /// Return parameter value or default value.
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? GetValueOrDefault<T>(string parameterName, T? defaultValue = default)
    {
        if (!TryGetValue(parameterName, out var value))
        {
            return defaultValue;
        }
        
        if (typeof(T) == typeof(string))
        {
            return (dynamic?) value;
        }

        return value is null 
            ? defaultValue
            : JsonSerializer.Deserialize<T>(value);
    }
}