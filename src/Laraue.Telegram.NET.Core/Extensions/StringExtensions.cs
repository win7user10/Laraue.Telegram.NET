﻿using Laraue.Telegram.NET.Core.Request;

namespace Laraue.Telegram.NET.Core.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Get full query string, e.g. path?param1=value1 and return all
    /// query params as a dictionary.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static RequestParameters ParseQueryParts(this string? source)
    {
        var result = new RequestParameters();
        
        if (source is null)
        {
            return result;
        }

        var sourceSpan = source.AsSpan();
        if (!sourceSpan.TrySplit('?', out _, out var parametersSpan))
        {
            return result;
        }

        while (true)
        {
            var isSplit = parametersSpan.TrySplit('&', out var parameterSpan, out parametersSpan);
            
            if (parameterSpan.TrySplit('=', out var leftSpan, out var rightSpan))
            {
                result[leftSpan.ToString()] = rightSpan.ToString();
            }

            if (!isSplit)
            {
                break;
            }
        }

        return result;
    }

    private static bool TrySplit(this ReadOnlySpan<char> source, char middleCharacter, out ReadOnlySpan<char> leftSpan, out ReadOnlySpan<char> rightSpan)
    {
        var middleCharacterIndex = source.IndexOf(middleCharacter);

        if (middleCharacterIndex == -1 || middleCharacterIndex > source.Length)
        {
            leftSpan = source;
            rightSpan = ReadOnlySpan<char>.Empty;
            return false;
        }

        leftSpan = source[..middleCharacterIndex];
        rightSpan = source[(middleCharacterIndex + 1)..];
        return true;
    }
}