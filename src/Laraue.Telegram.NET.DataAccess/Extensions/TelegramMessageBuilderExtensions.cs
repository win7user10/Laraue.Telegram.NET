using Laraue.Core.DataAccess.Contracts;
using Laraue.Telegram.NET.Core.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Laraue.Telegram.NET.DataAccess.Extensions;

public static class TelegramMessageBuilderExtensions
{
    private const string PageParameterName = "p";
    
    public static TelegramMessageBuilder AppendDataRows<TData>(
        this TelegramMessageBuilder messageBuilder,
        IPaginatedResult<TData> result,
        Func<TData, int, string> formatRow)
        where TData : class
    {
        foreach (var row in result.Data.Select(formatRow))
        {
            messageBuilder.AppendRow(row);
        }

        return messageBuilder;
    }
    
    public static TelegramMessageBuilder AddControlButtons<TData>(
        this TelegramMessageBuilder messageBuilder,
        IPaginatedResult<TData> result,
        string route)
        where TData : class
    {
        if (result is {HasPreviousPage: false, HasNextPage: false})
        {
            return messageBuilder;
        }

        var rowButtons = new List<InlineKeyboardButton>();
        if (result.HasPreviousPage)
        {
            rowButtons.Add(InlineKeyboardButton.WithCallbackData(
                "Previous ⬅",
                $"{route}?{PageParameterName}={result.Page - 1}"));
        }
        if (result.HasNextPage)
        {
            rowButtons.Add(InlineKeyboardButton.WithCallbackData(
                "Next ➡",
                $"{route}?{PageParameterName}={result.Page + 1}"));
        }

        messageBuilder.AddInlineKeyboardButtons(rowButtons);

        return messageBuilder;
    }

    public static TelegramMessageBuilder AddInlineKeyboardButtons<TData>(
        this TelegramMessageBuilder messageBuilder,
        IPaginatedResult<TData> result,
        Func<TData, int, InlineKeyboardButton> getButton)
        where TData : class
    {
        var rowButtons = result.Data
            .Select(getButton);

        return messageBuilder.AddInlineKeyboardButtons(rowButtons);
    }
}