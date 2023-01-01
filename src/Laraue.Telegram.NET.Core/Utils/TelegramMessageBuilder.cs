using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace Laraue.Telegram.NET.Core.Utils;

public class TelegramMessageBuilder
{
    private readonly List<string> _rows = new ();
    private readonly List<IEnumerable<InlineKeyboardButton>> _inlineKeyboardButtons = new ();
    private readonly List<IEnumerable<KeyboardButton>> _keyboardButtons = new ();

    public TelegramMessageBuilder AppendRow(string text)
    {
        _rows.Add(text);

        return this;
    }

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
    
    public TelegramMessageBuilder AddReplyKeyboardButtons(IEnumerable<KeyboardButton> rowButtons)
    {
        _keyboardButtons.Add(rowButtons);

        return this;
    }

    public string Text => string.Join("\n", _rows);
    public InlineKeyboardMarkup InlineKeyboard => new (_inlineKeyboardButtons);
    public ReplyKeyboardMarkup ReplyKeyboard => new (_keyboardButtons) { ResizeKeyboard = true };
}