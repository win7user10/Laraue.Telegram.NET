using System.Reflection;
using Laraue.Telegram.NET.Interceptors.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.Interceptors.EFCore.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add telegram request interceptors with EFCore storage implementation.
    /// Do not forget to apply 
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="interceptorAssemblies"></param>
    /// <typeparam name="TUserKey"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddTelegramRequestEfCoreInterceptors<TUserKey, TDbContext>(
        this IServiceCollection serviceCollection,
        IEnumerable<Assembly>? interceptorAssemblies = null)
        where TUserKey : IEquatable<TUserKey>
        where TDbContext : class, IInterceptorsDbContext<TUserKey>
    {
        serviceCollection.AddScoped<IInterceptorsDbContext<TUserKey>>(sp => sp.GetRequiredService<TDbContext>());
        
        return serviceCollection.AddTelegramRequestInterceptors<EFCoreInterceptorState<TUserKey>, TUserKey>(
            ServiceLifetime.Scoped,
            interceptorAssemblies);
    }
}