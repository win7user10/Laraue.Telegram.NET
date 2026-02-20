namespace Laraue.Telegram.NET.Core.Services;

public interface IExceptionHandler
{
    Task<bool> TryHandleAsync(
        Exception exception,
        CancellationToken cancellationToken = default);
}