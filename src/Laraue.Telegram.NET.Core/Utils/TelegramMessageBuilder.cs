using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace Laraue.Telegram.NET.Core.Utils;

/// <summary>
/// Message builder for telegram messages.
/// </summary>
public class TelegramMessageBuilder
{
    private readonly StringBuilder _textBuilder = new ();
    private readonly List<IEnumerable<InlineKeyboardButton>> _inlineKeyboardButtons = new ();
    private readonly List<IEnumerable<KeyboardButton>> _keyboardButtons = new ();

    /// <summary>
    /// Add text row to the message.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public TelegramMessageBuilder AppendRow(string text)
    {
        _textBuilder.AppendLine(text);

        return this;
    }
    
    /// <summary>
    /// Adds new empty line.
    /// </summary>
    /// <returns></returns>
    public TelegramMessageBuilder AppendRow()
    {
        _textBuilder.AppendLine();

        return this;
    }
    
    /// <summary>
    /// Append the passed string builder.
    /// </summary>
    public TelegramMessageBuilder Append(StringBuilder stringBuilder)
    {
        _textBuilder.Append(stringBuilder);

        return this;
    }
    
    /// <summary>
    /// Append the passed string.
    /// </summary>
    public TelegramMessageBuilder Append(string value)
    {
        _textBuilder.Append(value);

        return this;
    }

    /// <summary>
    /// Add buttons with sends callbacks when pressed to the message.
    /// </summary>
    /// <param name="rowButtons"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public TelegramMessageBuilder AddInlineKeyboardButtons(IEnumerable<InlineKeyboardButton> rowButtons)
    {
        var rowButtonsList = rowButtons.ToList();
        
        foreach (var rowButton in rowButtonsList)
        {
            var callbackDataLength = Encoding.UTF8.GetByteCount(rowButton.CallbackData ?? string.Empty);
            if (callbackDataLength > 64)
            {
                throw new InvalidOperationException($"The button with text '{rowButton.Text}' has callback '{rowButton.CallbackData}' with length {callbackDataLength}, <= 64 is required");
            }
        }       
        
        _inlineKeyboardButtons.Add(rowButtonsList);

        return this;
    }
    
    /// <summary>
    /// Add buttons that send simple text when pressed.
    /// </summary>
    /// <param name="rowButtons"></param>
    /// <returns></returns>
    public TelegramMessageBuilder AddReplyKeyboardButtons(IEnumerable<KeyboardButton> rowButtons)
    {
        _keyboardButtons.Add(rowButtons);

        return this;
    }

    /// <summary>
    /// Returns message text.
    /// </summary>
    public string Text => _textBuilder.ToString();
    
    /// <summary>
    /// Returns Keyboard with callback buttons.
    /// </summary>
    public InlineKeyboardMarkup InlineKeyboard => new (_inlineKeyboardButtons);
    
    /// <summary>
    /// Returns Keyboard with text buttons.
    /// </summary>
    public ReplyKeyboardMarkup ReplyKeyboard => new (_keyboardButtons) { ResizeKeyboard = true };
}