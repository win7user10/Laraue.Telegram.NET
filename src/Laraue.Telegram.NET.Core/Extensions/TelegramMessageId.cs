using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Fields to identify a message to edit it.
/// </summary>
public record TelegramMessageId(ChatId ChatId, int MessageId);