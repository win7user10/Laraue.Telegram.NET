namespace Laraue.Telegram.NET.Core.Routing;

public class RouteNotFoundException(long elapsedMilliseconds, object payload) : Exception
{
    public long ElapsedMilliseconds { get; } = elapsedMilliseconds;
    public object Payload { get; } = payload;
}