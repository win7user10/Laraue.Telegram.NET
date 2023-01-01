using System.Text.RegularExpressions;

namespace Laraue.Telegram.NET.Core.Routing;

public static class RouteRegexCreator
{
    public static Regex ForRoute(string path)
    {
        var regex = Regex.Replace(
            path,
            "{(\\w+)}", _ => $"([\\w|\\s]+)");
        
        return new Regex($"^{regex}");
    }
}