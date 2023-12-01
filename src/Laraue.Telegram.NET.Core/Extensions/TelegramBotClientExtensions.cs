using Laraue.Telegram.NET.Core.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Extensions for the <see cref="ITelegramBotClient"/>.
/// </summary>
public static class TelegramBotClientExtensions
{
    /// <summary>
    /// Sends telegram message builder content to a user.
    /// </summary>
    public static Task SendTextMessageAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        TelegramMessageBuilder messageBuilder,
        int? messageThreadId = default,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? entities = default,
        bool? disableWebPagePreview = default,
        bool? disableNotification = default,
        bool? protectContent = default,
        int? replyToMessageId = default,
        bool? allowSendingWithoutReply = default,
        CancellationToken cancellationToken = default)
    {
        return botClient.SendTextMessageAsync(
            chatId: chatId,
            text: messageBuilder.Text,
            messageThreadId: messageThreadId,
            parseMode: parseMode,
            entities: entities,
            disableWebPagePreview: disableWebPagePreview,
            disableNotification: disableNotification,
            protectContent: protectContent,
            replyToMessageId: replyToMessageId,
            allowSendingWithoutReply: allowSendingWithoutReply,
            replyMarkup: messageBuilder.InlineKeyboard,
            cancellationToken: cancellationToken);
    }
}