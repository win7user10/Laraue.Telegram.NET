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
    public TKey Id { get; set; }
    
    /// <summary>
    /// Telegram identifier.
    /// </summary>
    public long TelegramId { get; set; }

    /// <summary>
    /// Telegram user name.
    /// </summary>
    [MaxLength(32)]
    public string? TelegramUserName { get; set; }

    /// <summary>
    /// Telegram user language code, e.g "ru", "en" etc.
    /// </summary>
    [MaxLength(2)]
    public string? TelegramLanguageCode { get; set; }
    
    /// <summary>
    /// Telegram last name.
    /// </summary>
    [MaxLength(64)]
    public string? TelegramLastName { get; set; }
    
    /// <summary>
    /// Telegram first name.
    /// </summary>
    [MaxLength(64)]
    public string? TelegramFirstName { get; set; }

    /// <summary>
    /// When the user first time wrote to the telegram bot (in UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }
}