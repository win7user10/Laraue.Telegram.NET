namespace Laraue.Telegram.NET.Interceptors.Services;

/// <summary>
/// The class that will be executed in the request pipeline for the user instead of standard request pipeline. 
/// </summary>
public interface IRequestInterceptor
{
    /// <summary>
    /// Unique interceptor identifier. Each awaiter Type should have its own unique identifier. 
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Run the interceptor.
    /// </summary>
    /// <returns></returns>
    Task<ExecutionState> ExecuteAsync();
}

/// <summary>
/// Interceptor execution state.
/// </summary>
public enum ExecutionState
{
    /// <summary>
    /// Interceptor is fully executed and no more required.
    /// </summary>
    FullyExecuted,
    
    /// <summary>
    /// Interceptor was executed, but should be executed again, at the next request.
    /// </summary>
    ParticularlyExecuted,
    
    /// <summary>
    /// Interceptor execution was skipped, the next request should try to execute it again.
    /// </summary>
    NotExecuted,
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