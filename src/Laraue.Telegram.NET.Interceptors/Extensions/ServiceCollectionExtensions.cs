using System.Reflection;
using Laraue.Telegram.NET.Core.Extensions;
using Laraue.Telegram.NET.Interceptors.Middleware;
using Laraue.Telegram.NET.Interceptors.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.Interceptors.Extensions;

/// <summary>
/// Extensions to add AnswerToQuestion functionality to the container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add opportunity to "answer" on the routes.
    /// The next schema can be used:
    /// 1. Message route executes and asks something from the client. Ex: "What is your age?"
    /// 2. Message route should register response awaiting for the question.
    /// _responseAwaiter.RegisterAwaiter(IResponseAwaiter awaiter)
    /// 3. Next response will try to find registered awaiter and only if it was not found
    /// will execute usual routing, otherwise execute that awaiter.
    /// Should be registered before authentication middleware.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="storageLifetime"></param>
    /// <param name="interceptorAssemblies"></param>
    /// <typeparam name="TInterceptorState"></typeparam>
    /// <typeparam name="TUserKey"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddTelegramRequestInterceptors<TInterceptorState, TUserKey>(
        this IServiceCollection serviceCollection,
        ServiceLifetime storageLifetime = ServiceLifetime.Scoped,
        IEnumerable<Assembly>? interceptorAssemblies = null)
        where TInterceptorState : class, IInterceptorState<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        var interceptors = (interceptorAssemblies ?? new []{ Assembly.GetCallingAssembly() })
            .SelectMany(x => x.GetTypes())
            .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsAssignableTo(typeof(IRequestInterceptor)));

        foreach (var interceptor in interceptors)
        {
            serviceCollection.AddScoped(interceptor);
            serviceCollection.AddScoped(typeof(IRequestInterceptor), interceptor);
        }
        
        serviceCollection.AddTelegramMiddleware<InterceptorsMiddleware<TUserKey>>()
            .Add(new ServiceDescriptor(
                typeof(IInterceptorState<TUserKey>),
                typeof(TInterceptorState),
                storageLifetime));

        return serviceCollection;
    }
}