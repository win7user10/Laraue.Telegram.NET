namespace Laraue.Telegram.NET.Interceptors.Services;

/// <summary>
/// The class that will be executed in the request pipeline for the user instead if standard request pipeline. 
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

public interface IRequestInterceptor<TInterceptorContext> : IRequestInterceptor
{
}