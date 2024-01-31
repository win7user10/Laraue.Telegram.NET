using System.Globalization;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laraue.Telegram.NET.Localization;

/// <summary>
/// Base <see cref="ITelegramCultureInfoProvider"/> implementation with taking <see cref="CultureInfo"/>
/// from the received telegram request.
/// </summary>
public abstract class BaseCultureInfoProvider(
    TelegramRequestContext context,
    IOptions<TelegramRequestLocalizationOptions> options,
    ILogger<BaseCultureInfoProvider> logger)
    : ITelegramCultureInfoProvider
{
    /// <inheritdoc />
    public Task<TelegramProviderCultureResult?> DetermineProviderCultureResultAsync(
        CancellationToken cancellationToken = default)
    {
        var user = context.Update.GetUser();
        if (user is null)
        {
            return Task.FromResult((TelegramProviderCultureResult?)null);
        }
        
        var userInterfaceLanguage = user.LanguageCode;
        if (!options.Value.AvailableLanguages.Contains(userInterfaceLanguage) && options.Value.DefaultLanguage != userInterfaceLanguage)
        {
            logger.LogDebug(
                "Language {OldLanguageCode} is not supported, switch to {NewLanguageCode} for tg user {UserId}",
                userInterfaceLanguage,
                options.Value.DefaultLanguage,
                user.Id);
            
            userInterfaceLanguage = options.Value.DefaultLanguage;
        }
        
        if (userInterfaceLanguage is not null)
        {
            return DetermineProviderCultureResultAsync(
                new CultureInfo(userInterfaceLanguage),
                cancellationToken)!;
        }
        
        logger.LogDebug("Culture is missing for the tg user {UserId}", user.Id);
        return Task.FromResult((TelegramProviderCultureResult?)null);
    }

    /// <summary>
    /// Returns culture for the current request.
    /// </summary>
    /// <param name="userInterfaceCulture">Culture info of the telegram interface the request has been taken.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<TelegramProviderCultureResult> DetermineProviderCultureResultAsync(
        CultureInfo userInterfaceCulture,
        CancellationToken cancellationToken = default);
}