using Telegram.Bot.Requests.Abstractions;

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
        return item as T
            ?? throw new TelegramNetAssertException(
                $"Excepted request of type {typeof(T).FullName}, but received {item.GetType().FullName}");
    }
        
    /// <summary>
    /// Asserts that only one call has been made.
    /// </summary>
    /// <returns></returns>
    public IRequest Single()
    {
        return Source.Count switch
        {
            > 1 => throw new TelegramNetAssertException(
                $"Request collection contains more than one item ({Source.Count})"),
            0 => throw new TelegramNetAssertException($"Request collection contains zero items"),
            _ => Source[0]
        };
    }
}