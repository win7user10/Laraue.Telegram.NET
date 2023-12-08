using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Abstractions;

/// <summary>
/// The class that run routing logic in the Laraue.Telegram.Core package.
/// </summary>
public interface ITelegramRouter
{
    /// <summary>
    /// Handle telegram update and return something as response for message.
    /// </summary>
    /// <param name="update"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RouteAsync(Update update, CancellationToken cancellationToken = default);
}