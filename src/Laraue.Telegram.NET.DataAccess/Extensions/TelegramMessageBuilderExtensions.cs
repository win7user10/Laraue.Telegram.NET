using Laraue.Core.DataAccess.Contracts;
using Laraue.Telegram.NET.Core.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Laraue.Telegram.NET.DataAccess.Extensions;

/// <summary>
/// Extensions to build pagination functionality in the telegram views.
/// </summary>
public static class TelegramMessageBuilderExtensions
{
    private const string PageParameterName = "p";
    
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
    /// <param name="messageBuilder"></param>
    /// <param name="result"></param>
    /// <param name="route"></param>
    /// <param name="previousButtonText"></param>
    /// <param name="nextButtonText"></param>
    /// <param name="fallbackButtons"></param>
    /// <typeparam name="TData"></typeparam>
    /// <returns></returns>
    public static TelegramMessageBuilder AddPaginationButtons<TData>(
        this TelegramMessageBuilder messageBuilder,
        IShortPaginatedResult<TData> result,
        RoutePathBuilder route,
        string previousButtonText = "Previous ⬅",
        string nextButtonText = "Next ➡",
        ControlButtons? fallbackButtons = null)
        where TData : class
    {
        var rowButtons = new List<InlineKeyboardButton>();
        if (result.HasPreviousPage)
        {
            rowButtons.Add(InlineKeyboardButton.WithCallbackData(
                previousButtonText, 
                route.WithQueryParameter(PageParameterName, result.Page - 1)));
        }
        else if (fallbackButtons?.PreviousButton is not null)
        {
            rowButtons.Add(fallbackButtons.PreviousButton);
        }
        
        if (result.HasNextPage)
        {
            rowButtons.Add(InlineKeyboardButton.WithCallbackData(
                nextButtonText,
                route.WithQueryParameter(PageParameterName, result.Page + 1)));
        }
        else if (fallbackButtons?.NextButton is not null)
        {
            rowButtons.Add(fallbackButtons.NextButton);
        }

        if (rowButtons.Any())
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