using System.Reflection;
using Laraue.Telegram.NET.AnswerToQuestion.Middleware;
using Laraue.Telegram.NET.AnswerToQuestion.Services;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.AnswerToQuestion.Extensions;

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
    /// <param name="awaiterAssemblies"></param>
    /// <typeparam name="TQuestionStateStorage"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddAnswerToQuestionFunctionality<TQuestionStateStorage, TUserKey>(
        this IServiceCollection serviceCollection,
        ServiceLifetime storageLifetime = ServiceLifetime.Scoped,
        IEnumerable<Assembly>? awaiterAssemblies = null)
        where TQuestionStateStorage : class, IQuestionStateStorage<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        var responseAwaiters = (awaiterAssemblies ?? new []{ Assembly.GetCallingAssembly() })
            .SelectMany(x => x.GetTypes())
            .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsAssignableTo(typeof(IAnswerAwaiter)));

        foreach (var responseAwaiter in responseAwaiters)
        {
            serviceCollection.AddScoped(responseAwaiter);
            serviceCollection.AddScoped(typeof(IAnswerAwaiter), responseAwaiter);
        }
        
        serviceCollection.AddTelegramMiddleware<AnswerToQuestionMiddleware<TUserKey>>()
            .Add(new ServiceDescriptor(
                typeof(IQuestionStateStorage<TUserKey>),
                typeof(TQuestionStateStorage),
                storageLifetime));

        return serviceCollection;
    }
}