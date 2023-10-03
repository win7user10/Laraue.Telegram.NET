namespace Laraue.Telegram.NET.Abstractions.Request;

/// <summary>
/// Marks a parameter it should be taken from the query parameter from the route string. 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromQueryAttribute : Attribute
{
}