namespace Laraue.Telegram.NET.Abstractions;

/// <summary>
/// Additional logic to execute before/after request.
/// Get <see cref="ITelegramMiddleware"/> from the constructor to get link to the next middleware.
/// </summary>
public interface ITelegramMiddleware
{
    /// <summary>
    /// Execute middleware logic.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task InvokeAsync(CancellationToken ct = default);
}