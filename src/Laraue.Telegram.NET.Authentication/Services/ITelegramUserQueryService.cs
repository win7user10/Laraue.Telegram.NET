using Laraue.Telegram.NET.Authentication.Models;

namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Service that allows to retrieve information about system user in Telegram.NET. 
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TUserKey"></typeparam>
public interface ITelegramUserQueryService<TUser, TUserKey>
    where TUserKey : IEquatable<TUserKey>
    where TUser : class, ITelegramUser<TUserKey>, new()
{
    /// <summary>
    /// Find info about user by its telegram id. Returns null if user is not found.
    /// </summary>
    /// <param name="telegramId"></param>
    /// <returns></returns>
    Task<TUser?> FindAsync(long telegramId);
    
    /// <summary>
    /// Creates the new user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Identifier of the created user.</returns>
    Task<TUserKey> CreateAsync(TUser user);
}