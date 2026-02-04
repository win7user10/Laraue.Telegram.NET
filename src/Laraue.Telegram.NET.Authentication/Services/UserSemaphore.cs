using Laraue.Core.Threading;

namespace Laraue.Telegram.NET.Authentication.Services;

public interface IUserSemaphore
{
    Task<IDisposable> WaitAsync(long userId, CancellationToken cancellationToken = default);
}

public class UserSemaphore : IUserSemaphore
{
    private readonly KeyedSemaphoreSlim<long> _semaphore = new (1);
    
    public Task<IDisposable> WaitAsync(long userId, CancellationToken cancellationToken = default)
    {
        return _semaphore.WaitAsync(userId, cancellationToken);
    }
}