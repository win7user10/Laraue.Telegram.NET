using Microsoft.AspNetCore.Identity;

namespace Laraue.Telegram.NET.Authentication.Models;

/// <summary>
/// Model for user that can be registered from telegram.
/// </summary>
public class TelegramIdentityUser : IdentityUser
{
    /// <summary>
    /// Telegram identifier.
    /// </summary>
    public long? TelegramId { get; init; }
}