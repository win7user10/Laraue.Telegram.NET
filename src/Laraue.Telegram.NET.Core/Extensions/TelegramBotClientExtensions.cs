using Laraue.Telegram.NET.Core.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Extensions for the <see cref="ITelegramBotClient"/>.
/// </summary>
public static class TelegramBotClientExtensions
{
    /// <summary>
    /// Sends telegram message builder content to a user.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="chatId"></param>
    /// <param name="messageBuilder"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public static Task SendTextMessageAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        TelegramMessageBuilder messageBuilder,
        CancellationToken ct = default)
    {
        return botClient.SendTextMessageAsync(
            chatId,
            messageBuilder.Text,
            replyMarkup: messageBuilder.InlineKeyboard,
            cancellationToken: ct);
    }
}