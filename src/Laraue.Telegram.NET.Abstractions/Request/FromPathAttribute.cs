namespace Laraue.Telegram.NET.Abstractions.Request;

/// <summary>
/// Marks a parameter it should be taken from the path from the route string. 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromPathAttribute : Attribute
{
}