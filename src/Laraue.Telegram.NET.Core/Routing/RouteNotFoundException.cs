using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Routing;

public class RouteNotFoundException(long elapsedMilliseconds, Update payload) : Exception
{
    public long ElapsedMilliseconds { get; } = elapsedMilliseconds;
    public Update Payload { get; } = payload;
}