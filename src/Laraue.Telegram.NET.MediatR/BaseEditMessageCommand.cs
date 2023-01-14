using Laraue.Telegram.NET.Core.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Laraue.Telegram.NET.MediatR;

/// <summary>
/// The command to update previously sent message.
/// </summary>
/// <param name="Data"></param>
/// <param name="ChatId"></param>
/// <param name="MessageId"></param>
/// <param name="CallbackQueryId"></param>
/// <typeparam name="TData"></typeparam>
public record BaseEditMessageCommand<TData>(TData Data, long ChatId, int MessageId, string CallbackQueryId)
    : IRequest;

/// <summary>
/// Base message handler implementation.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TData"></typeparam>
public abstract class BaseEditMessageCommandHandler<TCommand, TData> : IRequestHandler<TCommand>
    where TCommand : BaseEditMessageCommand<TData>
{
    private readonly ITelegramBotClient _client;

    /// <summary>
    /// Initialize a new instance of <see cref="BaseEditMessageCommandHandler{TCommand,TData}"/>.
    /// </summary>
    /// <param name="client"></param>
    protected BaseEditMessageCommandHandler(ITelegramBotClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Edit telegram message with the passed command.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Unit> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var messageBuilder = new TelegramMessageBuilder();
        
        HandleInternal(request, messageBuilder);

        try
        {
            await _client.EditMessageTextAsync(
                request.ChatId,
                request.MessageId,
                messageBuilder.Text,
                ParseMode.Html,
                replyMarkup: messageBuilder.InlineKeyboard,
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException)
        {
            // Source does not modified.
        }

        try
        {
            await _client.AnswerCallbackQueryAsync(
                request.CallbackQueryId,
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException)
        {
            // Callback query is expired.
        }
        
        return Unit.Value;
    }
    
    /// <summary>
    /// Build telegram message using the passed command.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="telegramMessageBuilder"></param>
    protected abstract void HandleInternal(TCommand request, TelegramMessageBuilder telegramMessageBuilder);
}