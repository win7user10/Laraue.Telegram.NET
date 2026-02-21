namespace Laraue.Telegram.NET.Testing;

/// <summary>
/// The base exception for Telegram asserts.
/// </summary>
public class TelegramNetAssertException : Exception
{
    public TelegramNetAssertException(string message) : base(message)
    {
    }
}