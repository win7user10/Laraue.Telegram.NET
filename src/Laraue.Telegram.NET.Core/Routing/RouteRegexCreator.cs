using System.Text.RegularExpressions;

namespace Laraue.Telegram.NET.Core.Routing;

/// <summary>
/// Helper class to create regexes for path matching.
/// </summary>
public static partial class RouteRegexCreator
{
    /// <summary>
    /// Return the regex based on the passed string pattern.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Regex ForRoute(string path)
    {
        var replacedGreedyParameters = ReplaceGreedyParametersRegex().Replace(path, "(?<$1>(.*))");
        
        var replacedParameters = ReplaceParametersRegex().Replace(replacedGreedyParameters, "(?<$1>(\\w+))");
        
        return new Regex($"^{replacedParameters}(\\?.*)?$", RegexOptions.Compiled | RegexOptions.Multiline);
    }

    [GeneratedRegex(@"{(\w+)}\*")]
    private static partial Regex ReplaceGreedyParametersRegex();
    
    [GeneratedRegex("{(\\w+)}")]
    private static partial Regex ReplaceParametersRegex();
}