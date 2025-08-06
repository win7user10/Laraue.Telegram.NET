namespace Laraue.Telegram.NET.Core.LongPooling;

/// <summary>
/// Options for <see cref="LongPoolingTelegramBackgroundService"/>.
/// </summary>
public class LongPoolingOptions
{
    /// <summary>
    /// Used if last check returned updates.
    /// </summary>
    public int MinIntervalBetweenUpdatesCheckMs { get; set; } = 200;
    
    /// <summary>
    /// Used when each last update returned nothing.
    /// The value is automatically increases from <see cref="MinIntervalBetweenUpdatesCheckMs"/>
    /// with each received empty updates list.
    /// </summary>
    public int MaxIntervalBetweenUpdatesCheckMs { get; set; } = 3000;
    
    /// <summary>
    /// Batch size of updates requesting from telegram.
    /// </summary>
    public int BatchSize { get; set; } = 10;
}