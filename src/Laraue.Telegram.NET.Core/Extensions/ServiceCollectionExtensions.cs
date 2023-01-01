using System.Reflection;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Core.Routing;
using Laraue.Telegram.NET.Core.Routing.Attributes;
using Laraue.Telegram.NET.Core.Routing.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Laraue.Telegram.NET.Core.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds to the container telegram controllers.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="telegramBotClientOptions"></param>
    /// <param name="controllerAssemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramCore(
        this IServiceCollection serviceCollection,
        TelegramBotClientOptions telegramBotClientOptions,
        Assembly[]? controllerAssemblies = null)
    {
        serviceCollection.AddScoped<WebApplicationExtensions.MapRequestToTelegramCoreMiddleware>();
        
        serviceCollection
            .AddSingleton(telegramBotClientOptions)
            .AddSingleton<ITelegramBotClient, TelegramBotClient>()
            .AddScoped<ITelegramRouter, TelegramRouter>()
            .AddScoped<TelegramRequestContext>();

        serviceCollection.AddTelegramControllers(controllerAssemblies ?? new []{ Assembly.GetCallingAssembly() });
        serviceCollection.AddOptions<MiddlewareList>();
        serviceCollection.Configure<MiddlewareList>(opt =>
        {
            opt.AddToRoot<ExecuteRouteMiddleware>();
            opt.AddToTop<HandleExceptionsMiddleware>();
        });

        return serviceCollection;
    }

    /// <summary>
    /// Allows to add a custom middleware to the telegram request pipeline.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddTelegramMiddleware<TMiddleware>(this IServiceCollection serviceCollection)
        where TMiddleware : class, ITelegramMiddleware
    {
        serviceCollection.Configure<MiddlewareList>(middlewareList =>
        {
            middlewareList.Add<TMiddleware>();
        });

        return serviceCollection;
    }

    private static void AddTelegramControllers(this IServiceCollection serviceCollection, IEnumerable<Assembly> controllerAssemblies)
    {
        var controllerTypes = controllerAssemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsSubclassOf(typeof(TelegramController)));

        foreach (var controllerType in controllerTypes)
        {
            serviceCollection.AddScoped(controllerType);
            
            var methodInfos = controllerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.GetCustomAttribute<TelegramBaseRouteAttribute>(true) != null);
            
            foreach (var methodInfo in methodInfos)
            {
                var routeAttribute = methodInfo.GetCustomAttribute<TelegramBaseRouteAttribute>(true);
                if (routeAttribute is null)
                {
                    continue;
                }
                
                serviceCollection.AddSingleton<IRoute>(new Route(routeAttribute.IsMatch, methodInfo));
            }
        }
    }
}