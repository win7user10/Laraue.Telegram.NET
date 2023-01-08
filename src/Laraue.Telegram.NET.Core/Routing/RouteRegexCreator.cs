using System.Text.RegularExpressions;

namespace Laraue.Telegram.NET.Core.Routing;

public static class RouteRegexCreator
{
    public static Regex ForRoute(string path)
    {
        var regex = Regex.Replace(path, "\\*", ".*");
        
        return new Regex($"^{regex}$", RegexOptions.Compiled);
    }
}