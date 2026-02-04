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
    
    /// <summary>
    /// The count of messages to request when long pooling mode is used.
    /// </summary>
    public int? LongPoolingBatchSize { get; init; }
    
    /// <summary>
    /// The interval between long pooling cycles.
    /// </summary>
    public int? LongPoolingInterval { get; init; }
    
    /// <summary>
    /// Max telegram updates to load in memory queue from the storage.
    /// </summary>
    public int TelegramUpdatesInMemoryQueueMaxCount { get; init; }
    
    /// <summary>
    /// The interval between loading updates from storage.
    /// </summary>
    public int TelegramUpdatesPoolInterval { get; init; }
}