namespace Laraue.Telegram.NET.Interceptors.Services;

/// <summary>
/// The class that will be executed in the request pipeline for the user instead of standard request pipeline. 
/// </summary>
public interface IRequestInterceptor
{
    /// <summary>
    /// Unique awaiter identifier. Each awaiter Type should have its own unique identifier. 
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Try execute response awaiter if it is suitable for the execution.
    /// </summary>
    /// <returns></returns>
    Task<object?> ExecuteAsync();
}

/// <summary>
/// <see cref="IRequestInterceptor"/> with some context.
/// </summary>
/// <typeparam name="TInterceptorContext"></typeparam>
public interface IRequestInterceptor<in TInterceptorContext> : IRequestInterceptor
{
    /// <summary>
    /// Some kind of logic before interceptor will be set. E.g. ask user a question.
    /// </summary>
    /// <returns></returns>
    Task BeforeInterceptorSetAsync(TInterceptorContext? context);
}