namespace Laraue.Telegram.NET.Abstractions.Exceptions;

/// <summary>
/// This exception shows that some data passed from the user was invalid.
/// </summary>
public class BadTelegramRequestException : Exception
{
    /// <summary>
    /// Initializes a <see cref="BadTelegramRequestException"/> with the
    /// error message that will be send to the telegram user.
    /// </summary>
    /// <param name="message"></param>
    public BadTelegramRequestException(string message) : base(message)
    {}
}