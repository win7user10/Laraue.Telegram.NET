using System.Globalization;

namespace Laraue.Telegram.NET.Localization;

/// <summary>
/// Provider that returns locale for the current request.
/// </summary>
public interface ITelegramCultureInfoProvider
{
    /// <summary>
    /// Return culture result for the telegram request.
    /// </summary>
    /// <returns></returns>
    Task<TelegramProviderCultureResult?> DetermineProviderCultureResultAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Get locale result, like en-US.
/// </summary>
/// <param name="UiCulture">Strings culture</param>
/// <param name="Culture">Globalization settings (date, currencies) culture</param>
public sealed record TelegramProviderCultureResult(CultureInfo UiCulture, CultureInfo Culture);