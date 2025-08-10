namespace Laraue.Telegram.NET.Core;

/// <summary>
/// Options for Telegram.NET client.
/// </summary>
public class TelegramNetOptions
{
    /// <summary>
    /// Bot token.
    /// </summary>
    public required string Token { get; init; }
    
    /// <summary>
    /// Webhook url for the bot. When the field is filled webhook mode could be run,
    /// otherwise long pooling mode only supported.
    /// </summary>
    public string? WebhookUrl { get; init; }
}