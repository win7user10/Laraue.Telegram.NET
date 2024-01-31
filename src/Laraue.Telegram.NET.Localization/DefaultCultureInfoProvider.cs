using System.Globalization;
using Laraue.Telegram.NET.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laraue.Telegram.NET.Localization;

/// <summary>
/// Default implementation that always set culture equals to the passed with the request.
/// Real implementation should probably go to the DB to get saved culture independent from
/// the request locale.
/// </summary>
/// <param name="context"></param>
/// <param name="options"></param>
/// <param name="logger"></param>
public class DefaultCultureInfoProvider(
    TelegramRequestContext context,
    IOptions<TelegramRequestLocalizationOptions> options,
    ILogger<BaseCultureInfoProvider> logger)
    : BaseCultureInfoProvider(context, options, logger)
{
    /// <inheritdoc />
    protected override Task<TelegramProviderCultureResult> DetermineProviderCultureResultAsync(
        CultureInfo userInterfaceCulture,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TelegramProviderCultureResult(
            userInterfaceCulture,
            userInterfaceCulture));
    }
}