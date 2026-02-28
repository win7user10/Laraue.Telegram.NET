using System.Diagnostics.CodeAnalysis;
using Laraue.Telegram.NET.Authentication.Services;

namespace Laraue.Telegram.NET.Testing.Mocks;

public class MockUserIdByTelegramIdCache<TUserKey> : IUserIdByTelegramIdCache<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
    public Task<bool> TryGetValueAsync(long telegramId, [NotNullWhen(true)] out TUserKey? userId)
    {
        userId = default;
        return Task.FromResult(false);
    }

    public Task TryAddAsync(long telegramId, TUserKey userId)
    {
        return Task.CompletedTask;
    }
}