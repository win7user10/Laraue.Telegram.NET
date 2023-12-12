namespace Laraue.Telegram.NET.Abstractions.Request;

/// <summary>
/// Marks a parameter it should be taken from the path from the route string. 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromPathAttribute : Attribute
{
    /// <summary>
    /// Path property to take.
    /// </summary>
    public string? PropertyName { get; }
    
    /// <summary>
    /// Initializes a new instance of <see cref="FromQueryAttribute"/>.
    /// </summary>
    /// <param name="propertyName">When set, the value will taken from the path parameter with this key.</param>
    public FromPathAttribute(string? propertyName = null)
    {
        PropertyName = propertyName;
    }
}