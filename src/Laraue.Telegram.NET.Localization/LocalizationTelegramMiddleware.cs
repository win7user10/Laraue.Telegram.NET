﻿using System.Globalization;
using Laraue.Telegram.NET.Abstractions;

namespace Laraue.Telegram.NET.Localization;

/// <summary>
/// Sets thread culture based on culture returned from <see cref="ITelegramCultureInfoProvider"/>.
/// </summary>
/// <param name="next"></param>
/// <param name="provider"></param>
public class LocalizationTelegramMiddleware(ITelegramCultureInfoProvider provider)
    : ITelegramMiddleware
{
    /// <inheritdoc />
    public async Task InvokeAsync(Func<CancellationToken, Task> next, CancellationToken ct = default)
    {
        var requestCulture = await provider
            .DetermineProviderCultureResultAsync(ct)
            .ConfigureAwait(false);

        if (requestCulture is not null)
        {
            CultureInfo.CurrentUICulture = requestCulture.UiCulture;
            CultureInfo.CurrentCulture = requestCulture.Culture;
        }

        await next(ct).ConfigureAwait(false);
    }
}