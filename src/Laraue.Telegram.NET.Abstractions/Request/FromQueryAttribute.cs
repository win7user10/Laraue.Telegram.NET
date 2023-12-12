﻿namespace Laraue.Telegram.NET.Abstractions.Request;

/// <summary>
/// Marks a parameter it should be taken from the query parameter from the route string. 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromQueryAttribute : Attribute
{
    /// <summary>
    /// Query property to take.
    /// </summary>
    public string? PropertyName { get; }
    
    /// <summary>
    /// Initializes a new instance of <see cref="FromQueryAttribute"/>.
    /// </summary>
    /// <param name="propertyName">When set, the value will taken from the query parameter with this key.</param>
    public FromQueryAttribute(string? propertyName = null)
    {
        PropertyName = propertyName;
    }
}