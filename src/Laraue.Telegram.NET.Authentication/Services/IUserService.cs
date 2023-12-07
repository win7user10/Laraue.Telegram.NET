namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Contains methods to register or login telegram user.
/// </summary>
public interface IUserService<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Should return system id for the telegram user.
    /// </summary>
    Task<LoginResponse<TKey>> LoginOrRegisterAsync(TelegramData loginData);
}

/// <summary>
/// Login response with the system user id.
/// </summary>
public record LoginResponse<TKey>(TKey UserId) where TKey : IEquatable<TKey>;

/// <summary>
/// Telegram credentials to get system user id.
/// </summary>
public record TelegramData(long Id, string Username);