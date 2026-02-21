using Telegram.Bot.Requests.Abstractions;
using Xunit;

namespace Laraue.Telegram.NET.Testing;

/// <summary>
/// The list of made telegram API calls.
/// </summary>
/// <param name="requests"></param>
public class TelegramRequests(List<IRequest> requests)
{
    public IReadOnlyList<IRequest> Source { get; } = requests;

    /// <summary>
    /// Asserts that only one call has been made, and it's type is the same as passed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Single<T>() where T : class, IRequest
    {
        var item = Single();
        return Assert.IsType<T>(item);
    }
        
    /// <summary>
    /// Asserts that only one call has been made.
    /// </summary>
    /// <returns></returns>
    public IRequest Single()
    {
        return Assert.Single(Source);
    }
}