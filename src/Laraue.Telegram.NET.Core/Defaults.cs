using System.Text.Json;

namespace Laraue.Telegram.NET.Core;

/// <summary>
/// Default settings.
/// </summary>
public static class Defaults
{
    /// <summary>
    /// Default json options for all serializations.
    /// </summary>
    public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
}