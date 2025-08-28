using System.ComponentModel.DataAnnotations;

namespace Laraue.Telegram.NET.Authentication.Models;

/// <inheritdoc />
public interface ITelegramUser : ITelegramUser<string>
{
}

/// <summary>
/// Model for user that can be registered from telegram.
/// </summary>
public interface ITelegramUser<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// System user identifier.
    /// </summary>
    public TKey Id { get; init; }
    
    /// <summary>
    /// Telegram identifier.
    /// </summary>
    public long TelegramId { get; init; }

    /// <summary>
    /// Telegram user name.
    /// </summary>
    [MaxLength(32)]
    public string? TelegramUserName { get; init; }

    /// <summary>
    /// Telegram user language code, e.g "ru", "en" etc.
    /// </summary>
    [MaxLength(2)]
    public string? TelegramLanguageCode { get; init; }

    /// <summary>
    /// When the user first time wrote to the telegram bot (in UTC).
    /// </summary>
    public DateTime CreatedAt { get; init; }
}