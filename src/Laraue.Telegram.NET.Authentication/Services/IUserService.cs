namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Contains methods to register or login telegram user.
/// </summary>
public interface IUserService
{
    Task<LoginResponse> LoginAsync(LoginData loginData);
    Task<LoginResponse> RegisterAsync(LoginData loginData);
    Task<LoginResponse> LoginOrRegisterAsync(TelegramData loginData);
}

public record LoginResponse(string UserId);
public record LoginData(string Username, string Password);
public record TelegramData(long Id, string Username);