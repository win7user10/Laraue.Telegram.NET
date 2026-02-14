namespace Laraue.Telegram.NET.Authentication.Services;

public class InMemoryUserIdByTelegramIdCache<TUserId> : IUserIdByTelegramIdCache<TUserId> where TUserId : IEquatable<TUserId>
{
    private readonly Dictionary<long, TUserId> _data = new ();

    public Task<TUserId?> TryGetValueAsync(long telegramId)
    {
        _data.TryGetValue(telegramId, out var userId);
        
        return Task.FromResult(userId);
    }

    public Task TryAddAsync(long telegramId, TUserId userId)
    {
        _data.TryAdd(telegramId, userId);
        
        return Task.CompletedTask;
    }
}