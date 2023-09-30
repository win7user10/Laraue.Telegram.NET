using Telegram.Bot.Types.ReplyMarkups;

namespace Laraue.Telegram.NET.DataAccess.Extensions;

/// <summary>
/// The set of buttons to use for a navigation.
/// </summary>
/// <param name="PreviousButton"></param>
/// <param name="NextButton"></param>
public sealed record ControlButtons(InlineKeyboardButton? PreviousButton, InlineKeyboardButton? NextButton);