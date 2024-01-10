using Laraue.Telegram.NET.Core.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
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

    /// <summary>
    /// Edit telegram message with the content of the passed <see cref="TelegramMessageBuilder"/>.
    /// </summary>
    public static async Task EditMessageTextAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        int messageId,
        TelegramMessageBuilder messageBuilder,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? entities = default,
        bool? disableWebPagePreview = default,
        bool throwOnMessageNotModified = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: messageBuilder.Text,
                parseMode: parseMode,
                entities: entities,
                disableWebPagePreview: disableWebPagePreview,
                replyMarkup: messageBuilder.InlineKeyboard,
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException e) when (
            e.ErrorCode == 400 && e.Message.StartsWith("Bad Request: message is not modified"))
        {
            if (throwOnMessageNotModified)
            {
                throw;
            }
        }
    }
    
    /// <summary>
    /// Edit telegram message with the content of the passed <see cref="TelegramMessageBuilder"/>.
    /// </summary>
    public static Task EditMessageTextAsync(
        this ITelegramBotClient botClient,
        TelegramMessageId messageId,
        TelegramMessageBuilder messageBuilder,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? entities = default,
        bool? disableWebPagePreview = default,
        bool throwOnMessageNotModified = false,
        CancellationToken cancellationToken = default)
    {
        return botClient.EditMessageTextAsync(
            chatId: messageId.ChatId,
            messageId: messageId.MessageId,
            messageBuilder: messageBuilder,
            parseMode: parseMode,
            entities: entities,
            disableWebPagePreview: disableWebPagePreview,
            throwOnMessageNotModified: throwOnMessageNotModified,
            cancellationToken: cancellationToken);
    }
}