using Laraue.Telegram.NET.Abstractions;

namespace Laraue.Telegram.NET.Core.Routing.Middleware;

/// <summary>
/// Contains all middleware types in the request pipeline.
/// </summary>
public sealed class MiddlewareList
{
    private readonly Dictionary<byte, IList<Type>> _middlewareTypes = new ();

    internal void AddToRoot<TMiddleware>() where TMiddleware : class, ITelegramMiddleware
    {
        AddByPriority(1, typeof(TMiddleware));
    }
    
    internal void AddToTop<TMiddleware>() where TMiddleware : class, ITelegramMiddleware
    {
        AddByPriority(3, typeof(TMiddleware));
    }
    
    internal void Add<TMiddleware>() where TMiddleware : class, ITelegramMiddleware
    {
        AddByPriority(2, typeof(TMiddleware));
    }

    private void AddByPriority(byte index, Type middlewareType)
    {
        if (!_middlewareTypes.TryGetValue(index, out var value))
        {
            value = new List<Type>();
            _middlewareTypes[index] = value;
        }

        value.Add(middlewareType);
    }

    /// <summary>
    /// All middleware types in order they are executes.
    /// </summary>
    public IEnumerable<Type> Items
    {
        get
        {
            return _middlewareTypes
                .Keys
                .OrderBy(x => x)
                .SelectMany(x => _middlewareTypes[x]);
        }
    }
}