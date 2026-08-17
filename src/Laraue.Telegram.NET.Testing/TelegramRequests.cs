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
    /// Asserts that exactly one call of type <typeparamref name="T"/> was made, ignoring
    /// requests of other types, and returns it.
    /// </summary>
    public T Single<T>() where T : class, IRequest
    {
        var matches = OfType<T>();
        return matches.Count switch
        {
            > 1 => throw new TelegramNetAssertException(
                $"Expected exactly one request of type {typeof(T).FullName}, but found {matches.Count}"),
            0 => throw new TelegramNetAssertException(
                $"Expected exactly one request of type {typeof(T).FullName}, but found none"),
            _ => matches[0]
        };
    }

    /// <summary>
    /// Asserts that only one call has been made overall, regardless of type.
    /// </summary>
    public IRequest Single()
    {
        return Source.Count switch
        {
            > 1 => throw new TelegramNetAssertException(
                $"Request collection contains more than one item ({Source.Count})"),
            0 => throw new TelegramNetAssertException("Request collection contains zero items"),
            _ => Source[0]
        };
    }

    /// <summary>
    /// All calls of type <typeparamref name="T"/>, in call order.
    /// </summary>
    public IReadOnlyList<T> OfType<T>() where T : class, IRequest
        => Source.OfType<T>().ToList();

    /// <summary>
    /// The first call of type <typeparamref name="T"/>.
    /// </summary>
    public T First<T>() where T : class, IRequest
        => OfType<T>().FirstOrDefault()
            ?? throw new TelegramNetAssertException($"Expected a request of type {typeof(T).FullName}, but found none");

    /// <summary>
    /// The most recent call of type <typeparamref name="T"/> — handy for asserting the outcome of the latest step in a multi-callback flow.
    /// </summary>
    public T Last<T>() where T : class, IRequest
        => OfType<T>().LastOrDefault()
            ?? throw new TelegramNetAssertException($"Expected a request of type {typeof(T).FullName}, but found none");

    /// <summary>
    /// Number of calls of type <typeparamref name="T"/>.
    /// </summary>
    public int Count<T>() where T : class, IRequest
        => Source.OfType<T>().Count();

    /// <summary>
    /// Asserts no call of type <typeparamref name="T"/> was made — useful for proving a handler bailed out early (e.g. no picker shown after a rejection).
    /// </summary>
    public void None<T>() where T : class, IRequest
    {
        var count = Count<T>();
        if (count > 0)
            throw new TelegramNetAssertException($"Expected no requests of type {typeof(T).FullName}, but found {count}");
    }
}