using System.Numerics;
using System.Text.Json;

namespace Laraue.Telegram.NET.Abstractions.Request;

/// <summary>
/// Contains parameters dictionary where each parameter can
/// be retrieved and deserialized into the necessary type. 
/// </summary>
public class RequestParameters
{
    /// <summary>
    /// Request path parameters.
    /// </summary>
    private readonly Dictionary<string, string?> _pathParameters;
    
    /// <summary>
    /// Request query parameters.
    /// </summary>
    private readonly Dictionary<string, string?> _queryParameters;

    /// <summary>
    /// Initializes a new instance of <see cref="RequestParameters"/> with predefined values.
    /// </summary>
    /// <param name="pathParameters"></param>
    /// <param name="queryParameters"></param>
    public RequestParameters(IDictionary<string, string?> pathParameters, IDictionary<string, string?> queryParameters)
    {
        _pathParameters = new Dictionary<string, string?>(pathParameters, StringComparer.OrdinalIgnoreCase);
        _queryParameters = new Dictionary<string, string?>(queryParameters, StringComparer.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="RequestParameters"/> with predefined values.
    /// </summary>
    public RequestParameters()
    {
        _pathParameters = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        _queryParameters = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Return path parameter value.
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    public object? GetPathParameter(string parameterName, Type parameterType)
    {
        return GetParameter(_pathParameters, parameterName, parameterType);
    }
    
    /// <summary>
    /// Return query parameter value.
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    public object? GetQueryParameter(string parameterName, Type parameterType)
    {
        return GetParameter(_queryParameters, parameterName, parameterType);
    }

    /// <summary>
    /// Returns query parameters as an object of the selected type.
    /// </summary>
    /// <param name="modelType"></param>
    /// <returns></returns>
    public object GetQueryParameters(Type modelType)
    {
        return JsonSerializer.SerializeToDocument(_queryParameters)
            .Deserialize(modelType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
    
    private static object? GetParameter(IReadOnlyDictionary<string, string?> dictionary, string parameterName, Type valueType)
    {
        if (!dictionary.TryGetValue(parameterName, out var value))
        {
            return null;
        }

        if (value is null)
        {
            return null;
        }
        
        if (valueType == typeof(string))
        {
            return value;
        }

        valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;

        return valueType.IsEnum || valueType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INumber<>))
            ? JsonSerializer.Deserialize(value, valueType)
            : JsonSerializer.SerializeToElement(value).Deserialize(valueType);
    }
}