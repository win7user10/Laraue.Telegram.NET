using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Abstractions;

/// <summary>
/// Data connected with the current telegram request.
/// </summary>
public class TelegramRequestContext
{
    /// <summary>
    /// Telegram message associated with the current request.
    /// </summary>
    public Update Update { get; set; } = default!;

    /// <summary>
    /// Data associated with the request.
    /// </summary>
    public TelegramRequestContextData Data { get; } = new();
}

/// <summary>
/// Dictionary with parameters of context, which can be modified in the request pipeline.
/// </summary>
public class TelegramRequestContextData : Dictionary<string, object?>
{}