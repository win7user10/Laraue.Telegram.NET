using Laraue.Telegram.NET.Abstractions.Exceptions;
using Laraue.Telegram.NET.Authentication.Services;

namespace Laraue.Telegram.NET.Interceptors.Services;

/// <summary>
/// The common case of interceptor is to
/// 1. Take a request from a user
/// 2. Validate the passed data
/// 3. Show message that data is not valid, if validation is not passed
/// 4. Process the request if data is valid. 
/// </summary>
/// <typeparam name="TUserKey">Key type of the user in the system.</typeparam>
/// <typeparam name="TInput">Type of data that required to execute the request.</typeparam>
/// <typeparam name="TContext">Context data of the interceptor that can be used while it is executing.</typeparam>
public abstract class BaseRequestInterceptor<TUserKey, TInput, TContext> : IRequestInterceptor<TContext>
    where TUserKey : IEquatable<TUserKey>
    where TContext : class
{
    private readonly TelegramRequestContext<TUserKey> _requestContext;
    private readonly IInterceptorState<TUserKey> _interceptorState;

    protected BaseRequestInterceptor(
        TelegramRequestContext<TUserKey> requestContext,
        IInterceptorState<TUserKey> interceptorState)
    {
        _requestContext = requestContext;
        _interceptorState = interceptorState;
    }
    
    /// <inheritdoc />
    public abstract string Id { get; }

    /// <inheritdoc />
    public async Task<ExecutionState> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var interceptResult = new InterceptResult<TInput>();

        var context = await _interceptorState
            .GetInterceptorContextAsync<TContext>(
                _requestContext.GetUserIdOrThrow(),
                cancellationToken)
            .ConfigureAwait(false);

        if (context is null)
            throw new InvalidOperationException("Interceptor context hasn't been loaded");

        await ValidateAsync(
                _requestContext,
                interceptResult,
                context,
                cancellationToken)
            .ConfigureAwait(false);

        if (interceptResult.IsCancelled)
            return ExecutionState.Cancelled;
        
        if (interceptResult.Error is not null)
            throw new BadTelegramRequestException(interceptResult.Error);
        
        if (!interceptResult.IsResultSet)
            throw new InvalidOperationException("Model should be bind if validation finished successfully.");

        return await ExecuteRouteAsync(
                _requestContext,
                interceptResult.Model!,
                context,
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual Task BeforeInterceptorSetAsync(
        TContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// In this method validation should be passed.
    /// If any error occured, <see cref="InterceptResult{TResult}"/> should had an error.
    /// If errors are not occured <see cref="InterceptResult{TResult}.Model"/> in the results should be bind.
    /// </summary>
    protected abstract Task ValidateAsync(
        TelegramRequestContext<TUserKey> requestContext,
        InterceptResult<TInput> interceptResult,
        TContext interceptorContext,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Execute the awaiter body if validation has been passed successfully.
    /// </summary>
    /// <returns></returns>
    protected abstract Task<ExecutionState> ExecuteRouteAsync(
        TelegramRequestContext<TUserKey> requestContext,
        TInput model,
        TContext interceptorContext,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Interceptor that not requires a context.
/// </summary>
/// <typeparam name="TUserKey"></typeparam>
/// <typeparam name="TInput"></typeparam>
public abstract class BaseRequestInterceptor<TUserKey, TInput> 
    : BaseRequestInterceptor<TUserKey, TInput, EmptyContext>
    where TUserKey : IEquatable<TUserKey>
{
    /// <inheritdoc />
    protected BaseRequestInterceptor(
        TelegramRequestContext<TUserKey> requestContext,
        IInterceptorState<TUserKey> interceptorState)
        : base(requestContext, interceptorState)
    {
    }
}

/// <summary>
/// Context without state.
/// </summary>
public sealed class EmptyContext
{
    /// <summary>
    /// Context singleton value.
    /// </summary>
    public static EmptyContext Value { get; } = new ();
}