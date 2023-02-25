using Laraue.Telegram.NET.Abstractions;

namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Data connected with current telegram request context
/// with information about current user. 
/// </summary>
/// <typeparam name="TKey"></typeparam>
public sealed class TelegramRequestContext<TKey> : TelegramRequestContext
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Current user id identifier.
    /// </summary>
    public TKey? UserId { get; set; }
}