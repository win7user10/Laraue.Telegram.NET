using Laraue.Core.DataAccess.Contracts;
using Laraue.Telegram.NET.Core.Routing;
using Laraue.Telegram.NET.Core.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Laraue.Telegram.NET.DataAccess.Extensions;

/// <summary>
/// Extensions to build pagination functionality in the telegram views.
/// </summary>
public static class TelegramMessageBuilderExtensions
{
    /// <summary>
    /// Get the paginated result and format it to the string message.
    /// Each item will represent one row in the message.
    /// </summary>
    /// <param name="messageBuilder"></param>
    /// <param name="result"></param>
    /// <param name="formatRow"></param>
    /// <typeparam name="TData"></typeparam>
    /// <returns></returns>
    public static TelegramMessageBuilder AppendDataRows<TData>(
        this TelegramMessageBuilder messageBuilder,
        IShortPaginatedResult<TData> result,
        Func<TData, int, string> formatRow)
        where TData : class
    {
        foreach (var row in result.Data.Select(formatRow))
        {
            messageBuilder.AppendRow(row);
        }

        return messageBuilder;
    }
    
    /// <summary>
    /// Add buttons to move forward and back in the pagination telegram view.
    /// </summary>
    public static TelegramMessageBuilder AddPaginationButtons<TData>(
        this TelegramMessageBuilder messageBuilder,
        IShortPaginatedResult<TData> result,
        CallbackRoutePath callbackRoute,
        string previousButtonText = "Previous ⬅",
        string nextButtonText = "Next ➡",
        string pageParameterName = Defaults.PageParameterName,
        ControlButtons? fallbackButtons = null)
        where TData : class
    {
        var rowButtons = new List<InlineKeyboardButton>();
        if (result.HasPreviousPage)
        {
            rowButtons.Add(callbackRoute
                .WithQueryParameter(pageParameterName, result.Page - 1)
                .ToInlineKeyboardButton(previousButtonText));
        }
        else if (fallbackButtons?.PreviousButton is not null)
        {
            rowButtons.Add(fallbackButtons.PreviousButton);
        }
        
        if (result.HasNextPage)
        {
            rowButtons.Add(callbackRoute
                .WithQueryParameter(pageParameterName, result.Page + 1)
                .ToInlineKeyboardButton(nextButtonText));
        }
        else if (fallbackButtons?.NextButton is not null)
        {
            rowButtons.Add(fallbackButtons.NextButton);
        }

        if (rowButtons.Count != 0)
        {
            messageBuilder.AddInlineKeyboardButtons(rowButtons);
        }

        return messageBuilder;
    }

    /// <summary>
    /// Add inline button for each item in pagination result.
    /// </summary>
    /// <param name="messageBuilder"></param>
    /// <param name="result"></param>
    /// <param name="getButton"></param>
    /// <typeparam name="TData"></typeparam>
    /// <returns></returns>
    public static TelegramMessageBuilder AddInlineKeyboardButtons<TData>(
        this TelegramMessageBuilder messageBuilder,
        IShortPaginatedResult<TData> result,
        Func<TData, int, InlineKeyboardButton> getButton)
        where TData : class
    {
        var rowButtons = result.Data
            .Select(getButton);

        return messageBuilder.AddInlineKeyboardButtons(rowButtons);
    }
}