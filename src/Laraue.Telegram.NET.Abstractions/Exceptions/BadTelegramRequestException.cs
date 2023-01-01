namespace Laraue.Telegram.NET.Abstractions.Exceptions;

public class BadTelegramRequestException : Exception
{
    public BadTelegramRequestException(string message) : base(message)
    {}
}