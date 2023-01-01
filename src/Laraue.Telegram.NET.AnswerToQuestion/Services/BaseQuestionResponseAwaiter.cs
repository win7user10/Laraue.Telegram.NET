using System.Diagnostics.CodeAnalysis;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Abstractions.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

public abstract class BaseQuestionResponseAwaiter : IQuestionResponseAwaiter
{
    public async Task<object?> ExecuteAsync(IServiceProvider serviceProvider)
    {
        var telegramRequestContext = serviceProvider.GetRequiredService<TelegramRequestContext>();

        if (!TryValidate(telegramRequestContext.Update, out var error))
        {
            throw new BadTelegramRequestException(error);
        }

        return await ExecuteRouteAsync(telegramRequestContext);
    }

    protected abstract bool TryValidate(Update update, [NotNullWhen(false)] out string? errorMessage);
    
    protected abstract Task<object?> ExecuteRouteAsync(TelegramRequestContext telegramRequestContext);
}