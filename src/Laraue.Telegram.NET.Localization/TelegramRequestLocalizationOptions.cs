namespace Laraue.Telegram.NET.Localization;

/// <summary>
/// Options to setup <see cref="BaseCultureInfoProvider"/>.
/// </summary>
public sealed class TelegramRequestLocalizationOptions
{
    /// <summary>
    /// Setup available languages list for the request.
    /// If the request has been made from another language,
    /// the switch to <see cref="DefaultLanguage"/> will be made.
    /// </summary>
    public string[] AvailableLanguages { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Fallback language.
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";
}