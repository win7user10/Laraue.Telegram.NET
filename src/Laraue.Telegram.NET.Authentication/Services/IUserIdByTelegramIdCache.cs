namespace Laraue.Telegram.NET.Authentication.Services;

public interface IUserIdByTelegramIdCache<TUserId> where TUserId : IEquatable<TUserId>
{
    Task<TUserId?> TryGetValueAsync(long telegramId);

    Task TryAddAsync(long telegramId, TUserId userId);
}