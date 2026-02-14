using System.Diagnostics.CodeAnalysis;

namespace Laraue.Telegram.NET.Authentication.Services;

public interface IUserIdByTelegramIdCache<TUserId> where TUserId : IEquatable<TUserId>
{
    Task<bool> TryGetValueAsync(long telegramId, [NotNullWhen(true)] out TUserId? userId);

    Task TryAddAsync(long telegramId, TUserId userId);
}