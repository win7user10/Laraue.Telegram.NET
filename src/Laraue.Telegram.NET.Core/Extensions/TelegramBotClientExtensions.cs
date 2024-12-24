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
        ParseMode parseMode = default,
        ReplyParameters? replyParameters = default,
        LinkPreviewOptions? linkPreviewOptions = default,
        int? messageThreadId = default,
        IEnumerable<MessageEntity>? entities = default,
        bool disableNotification = default,
        bool protectContent = default,
        string? messageEffectId = default,
        string? businessConnectionId = default,
        bool allowPaidBroadcast = default,
        CancellationToken cancellationToken = default)
    {
        return botClient.SendMessage(
            chatId: chatId,
            text: messageBuilder.Text,
            messageThreadId: messageThreadId,
            replyParameters: replyParameters,
            linkPreviewOptions: linkPreviewOptions,
            parseMode: parseMode,
            entities: entities,
            disableNotification: disableNotification,
            protectContent: protectContent,
            replyMarkup: messageBuilder.InlineKeyboard,
            messageEffectId: messageEffectId,
            businessConnectionId: businessConnectionId,
            allowPaidBroadcast: allowPaidBroadcast,
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
        ParseMode parseMode = ParseMode.None,
        IEnumerable<MessageEntity>? entities = default,
        bool throwOnMessageNotModified = false,
        string? businessConnectionId = default,
        LinkPreviewOptions? linkPreviewOptions = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await botClient.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: messageBuilder.Text,
                parseMode: parseMode,
                entities: entities,
                linkPreviewOptions: linkPreviewOptions,
                replyMarkup: messageBuilder.InlineKeyboard,
                businessConnectionId: businessConnectionId,
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
        ParseMode parseMode = ParseMode.None,
        IEnumerable<MessageEntity>? entities = default,
        bool throwOnMessageNotModified = false,
        string? businessConnectionId = default,
        LinkPreviewOptions? linkPreviewOptions = default,
        CancellationToken cancellationToken = default)
    {
        return botClient.EditMessageTextAsync(
            chatId: messageId.ChatId,
            messageId: messageId.MessageId,
            messageBuilder: messageBuilder,
            parseMode: parseMode,
            entities: entities,
            linkPreviewOptions: linkPreviewOptions,
            throwOnMessageNotModified: throwOnMessageNotModified,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken);
    }
}