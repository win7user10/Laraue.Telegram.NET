using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Extensions for <see cref="CallbackQuery"/>.
/// </summary>
public static class CallbackQueryExtensions
{
    /// <summary>
    /// Returns message identifier from the message related to the callback query.
    /// </summary>
    /// <param name="callbackQuery"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static int GetMessageId(this CallbackQuery? callbackQuery)
    {
        return callbackQuery?.Message?.MessageId
            ?? throw new InvalidOperationException("Callback query is not passed");
    }
}