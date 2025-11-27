using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Utils;

/// <summary>
/// Telegram updates serializer for the logs.
/// </summary>
public static class TelegramUpdateSerializer
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };
    
    /// <summary>
    /// Serialize <see cref="Update"/> without storing unfilled properties.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public static string Serialize(Update update)
    {
        return JsonSerializer.Serialize(update, JsonSerializerOptions);
    }
}