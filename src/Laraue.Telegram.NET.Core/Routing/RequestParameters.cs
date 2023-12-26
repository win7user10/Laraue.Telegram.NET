using System.Reflection;
using System.Text.Json;
using Laraue.Telegram.NET.Abstractions.Request;

namespace Laraue.Telegram.NET.Core.Routing;

/// <summary>
/// Contains parameters dictionary where each parameter can
/// be retrieved and deserialized into the necessary type. 
/// </summary>
public class RequestParameters
{
    /// <summary>
    /// Request path parameters.
    /// </summary>
    private readonly Dictionary<string, string> _pathParameters;
    
    /// <summary>
    /// Request query parameters.
    /// </summary>
    private readonly Dictionary<string, string> _queryParameters;

    /// <summary>
    /// Cache for properties taken by reflection.
    /// </summary>
    private static readonly IDictionary<Type, IDictionary<string, PropertyCache>> PropertiesCache
        = new Dictionary<Type, IDictionary<string, PropertyCache>>();

    /// <summary>
    /// Initializes a new instance of <see cref="RequestParameters"/> with predefined values.
    /// </summary>
    /// <param name="pathParameters"></param>
    /// <param name="queryParameters"></param>
    public RequestParameters(IDictionary<string, string> pathParameters, IDictionary<string, string> queryParameters)
    {
        _pathParameters = new Dictionary<string, string>(pathParameters, StringComparer.OrdinalIgnoreCase);
        _queryParameters = new Dictionary<string, string>(queryParameters, StringComparer.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="RequestParameters"/> with predefined values.
    /// </summary>
    public RequestParameters()
    {
        _pathParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _queryParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
        using var s = new MemoryStream();
        var w = new Utf8JsonWriter(s);

        if (!PropertiesCache.TryGetValue(modelType, out var properties))
        {
            properties = modelType
                .GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance)
                .Select(x => new
                {
                    x.Name,
                    QueryAttribute = x.GetCustomAttribute<FromQueryAttribute>(),
                })
                .ToDictionary(
                    x => x.Name,
                    x => new PropertyCache
                    {
                        BindRequired = x.QueryAttribute?.BindRequired ?? false,
                        PropertyName = x.QueryAttribute?.PropertyName
                    },
                    StringComparer.OrdinalIgnoreCase);

            PropertiesCache[modelType] = properties;
        }
        
        w.WriteStartObject();
        foreach (var property in properties)
        {
            var propertyName = property.Value.PropertyName ?? property.Key; 
            if (!_queryParameters.TryGetValue(propertyName, out var queryParameter) && property.Value.BindRequired)
            {
                throw new BindException(property.Key);
            }

            if (queryParameter is null)
            {
                continue;
            }
            
            w.WritePropertyName(property.Key);
            w.WriteRawValue(queryParameter);
        }
        
        w.WriteEndObject();
        w.Flush();

        s.Seek(0, SeekOrigin.Begin);
        return JsonSerializer.Deserialize(s, modelType, Defaults.JsonOptions)!;
    }
    
    private static object? GetParameter(IReadOnlyDictionary<string, string> dictionary, string parameterName, Type valueType)
    {
        if (!dictionary.TryGetValue(parameterName, out var value))
        {
            return null;
        }
        
        return JsonSerializer.Deserialize(value, valueType, Defaults.JsonOptions);
    }
    
    private sealed class PropertyCache
    {
        public bool BindRequired { get; set; }

        public string? PropertyName { get; set; }
    }
}