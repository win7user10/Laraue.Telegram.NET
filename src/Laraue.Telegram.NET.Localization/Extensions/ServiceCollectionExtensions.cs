using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.Localization.Extensions;

/// <summary>
/// Extensions to add Localization functionality to the container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds localization middleware to the telegram pipeline.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <typeparam name="TCultureInfoProvider"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddTelegramRequestLocalization<TCultureInfoProvider>(
        this IServiceCollection serviceCollection)
        where TCultureInfoProvider : class, ITelegramCultureInfoProvider
    {
        return serviceCollection
            .AddTelegramMiddleware<LocalizationTelegramMiddleware>()
            .AddScoped<ITelegramCultureInfoProvider, TCultureInfoProvider>();
    }
    
    /// <summary>
    /// Adds localization middleware to the telegram pipeline with <see cref="DefaultCultureInfoProvider"/>
    /// as culture info provider.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramRequestLocalization(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddTelegramRequestLocalization<DefaultCultureInfoProvider>();
    }
}