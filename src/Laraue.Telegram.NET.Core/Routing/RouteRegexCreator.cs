using System.Text.RegularExpressions;

namespace Laraue.Telegram.NET.Core.Routing;

/// <summary>
/// Helper class to create regexes for path matching.
/// </summary>
public static class RouteRegexCreator
{
    /// <summary>
    /// Return the regex based on the passed string pattern.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Regex ForRoute(string path)
    {
        var regex = Regex.Replace(path, "{(\\w+)}", "(?<$1>(\\w+))");
        
        return new Regex($"^{regex}(\\?.*)?$", RegexOptions.Compiled);
    }
}