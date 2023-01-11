using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Abstractions;

/// <summary>
/// Data connected with the current telegram request.
/// </summary>
public sealed class TelegramRequestContext
{
    /// <summary>
    /// Telegram message associated with the current request.
    /// </summary>
    public Update Update { get; set; } = default!;

    /// <summary>
    /// Dictionary with parameters for pipeline customization.
    /// </summary>
    public Dictionary<string, object?> AdditionalParameters { get; } = new();

    /// <summary>
    /// Contains route that was executed in the current request.
    /// </summary>
    public IRoute? ExecutedRoute { get; set; }
    
    /// <summary>
    /// Current user id identifier.
    /// </summary>
    public string? UserId { get; set; }
}