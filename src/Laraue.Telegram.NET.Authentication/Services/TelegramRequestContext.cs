﻿using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Authentication.Middleware;

namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Data connected with current telegram request context
/// with information about current user. 
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class TelegramRequestContext<TKey> : TelegramRequestContext
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Current user id identifier.
    /// </summary>
    public TKey? UserId { get; set; }

    /// <summary>
    /// Gets current user id identifier or throw. 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public TKey GetUserIdOrThrow()
    {
        if (UserId == null || UserId.Equals(default))
        {
            throw new InvalidOperationException(
                $"User information is not available." +
                $" Ensure {typeof(AuthTelegramMiddleware<TKey>)} has been registered at the top of pipeline");
        }

        return UserId;
    }

    /// <summary>
    /// User groups list.
    /// </summary>
    public string[] Groups { get; set; } = Array.Empty<string>();
}