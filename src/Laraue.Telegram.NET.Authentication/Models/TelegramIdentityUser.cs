using Microsoft.AspNetCore.Identity;

namespace Laraue.Telegram.NET.Authentication.Models;

/// <summary>
/// Model for user that can be registered from telegram.
/// </summary>
public class TelegramIdentityUser : TelegramIdentityUser<string>
{
}

public class TelegramIdentityUser<TKey> : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Telegram identifier.
    /// </summary>
    public long? TelegramId { get; init; }

    /// <summary>
    /// Telegram user name.
    /// </summary>
    public string? TelegramUserName { get; init; }

    /// <summary>
    /// Telegram user language code, e.g "ru", "en" etc.
    /// </summary>
    public string? TelegramLanguageCode { get; init; }

    /// <summary>
    /// When the user first time wrote to the telegram bot (in UTC).
    /// </summary>
    public DateTime CreatedAt { get; init; }
}