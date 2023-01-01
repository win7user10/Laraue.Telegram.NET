using Laraue.Telegram.NET.Core.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
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
    private readonly ILogger<TCommand> _logger;

    protected BaseEditMessageCommandHandler(ITelegramBotClient client, ILogger<TCommand> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Unit> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var messageBuilder = new TelegramMessageBuilder();
        
        HandleInternal(request, messageBuilder);
        
        await _client.EditMessageTextAsync(
            request.ChatId,
            request.MessageId,
            messageBuilder.Text,
            ParseMode.Html,
            replyMarkup: messageBuilder.InlineKeyboard,
            cancellationToken: cancellationToken);

        try
        {
            await _client.AnswerCallbackQueryAsync(
                request.CallbackQueryId,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Error while answering to the callback query.");
        }
        
        return Unit.Value;
    }
    
    protected abstract void HandleInternal(TCommand request, TelegramMessageBuilder telegramMessageBuilder);
}