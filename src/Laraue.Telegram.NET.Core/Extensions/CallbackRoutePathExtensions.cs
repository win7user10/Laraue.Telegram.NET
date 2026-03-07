using System.Reflection;
using Laraue.Telegram.NET.Abstractions.Request;
using Laraue.Telegram.NET.Core.Routing;

namespace Laraue.Telegram.NET.Core.Extensions;

public static class CallbackRoutePathExtensions
{
    /// <summary>
    /// Add all properties of the object as query parameters.
    /// The property name is taken from <see cref="FromQueryAttribute"/> if exists.
    /// Otherwise, the class property name is taken.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="object"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static CallbackRoutePath WithQueryParameters<T>(
        this CallbackRoutePath path,
        T @object)
    {
        var properties = typeof(T).GetProperties(
            BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var queryAttribute = property.GetCustomAttribute<FromQueryAttribute>();
            var propertyName = queryAttribute?.PropertyName ?? property.Name;
            
            var value = property.GetValue(@object);
            path.WithQueryParameter(propertyName, value);
        }

        return path;
    }
}