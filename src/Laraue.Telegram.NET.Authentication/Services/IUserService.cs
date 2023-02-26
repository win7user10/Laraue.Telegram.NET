namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Contains methods to register or login telegram user.
/// </summary>
public interface IUserService<TKey> where TKey : IEquatable<TKey>
{
    Task<LoginResponse<TKey>> LoginAsync(LoginData loginData);
    Task<LoginResponse<TKey>> RegisterAsync(LoginData loginData);
    Task<LoginResponse<TKey>> LoginOrRegisterAsync(TelegramData loginData);
}

public record LoginResponse<TKey>(TKey UserId) where TKey : IEquatable<TKey>;
public record LoginData(string Username, string Password);
public record TelegramData(long Id, string Username);