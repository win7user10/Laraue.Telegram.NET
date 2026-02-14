using System.Diagnostics.CodeAnalysis;

namespace Laraue.Telegram.NET.Authentication.Services;

public class InMemoryUserIdByTelegramIdCache<TUserId> : IUserIdByTelegramIdCache<TUserId> where TUserId : IEquatable<TUserId>
{
    private readonly Dictionary<long, TUserId> _data = new ();

    public Task<bool> TryGetValueAsync(long telegramId, [NotNullWhen(true)] out TUserId? userId)
    {
        return Task.FromResult(_data.TryGetValue(telegramId, out userId));
    }

    public Task TryAddAsync(long telegramId, TUserId userId)
    {
        _data.TryAdd(telegramId, userId);
        
        return Task.CompletedTask;
    }
}