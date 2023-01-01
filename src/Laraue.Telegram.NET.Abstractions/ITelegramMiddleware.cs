namespace Laraue.Telegram.NET.Abstractions;

/// <summary>
/// Additional logic to execute before/after request.
/// </summary>
public interface ITelegramMiddleware
{
    public Task<object?> InvokeAsync(CancellationToken ct = default);
}