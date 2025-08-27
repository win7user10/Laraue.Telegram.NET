using Laraue.Telegram.NET.Authentication.Models;

namespace Laraue.Telegram.NET.Authentication.Services;

public class UserService<TUser, TKey> : IUserService<TKey>
    where TUser : class, ITelegramUser<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private readonly ITelegramUserQueryService<TUser, TKey> _telegramUserQueryService;

    public UserService(
        ITelegramUserQueryService<TUser, TKey> telegramUserQueryService)
    {
        _telegramUserQueryService = telegramUserQueryService;
    }
    
    /// <inheritdoc />
    public async Task<LoginResponse<TKey>> LoginOrRegisterAsync(TelegramData telegramData)
    {
        var user = await _telegramUserQueryService.FindAsync(telegramData.Id);
        if (user is not null)
        {
            return new LoginResponse<TKey>(user.Id);
        }
        
        var userId = await CreateUserInternalAsync(
            telegramData.Id,
            telegramData.Username,
            telegramData.LanguageCode);
        
        return new LoginResponse<TKey>(userId);
    }

    private Task<TKey> CreateUserInternalAsync(
        long telegramId,
        string? telegramUserName,
        string? telegramLanguageCode)
    {
        return _telegramUserQueryService.CreateAsync(
            new TUser
            {
                TelegramId = telegramId,
                TelegramUserName = telegramUserName,
                CreatedAt = DateTime.UtcNow,
                TelegramLanguageCode = telegramLanguageCode
            });
    }
}