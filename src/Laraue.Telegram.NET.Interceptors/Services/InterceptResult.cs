using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Interceptors.Services;

/// <summary>
/// <see cref="BaseRequestInterceptor{TUserKey,TModel}"/> validation result.
/// </summary>
/// <typeparam name="TResult">Awaiter model data type.</typeparam>
public sealed record InterceptResult<TResult>
{
    /// <summary>
    /// The model bind from the <see cref="Update"/> request.
    /// </summary>
    public TResult? Model { get; private set; }
    
    /// <summary>
    /// Error if the validation failed.
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Set the model received after validation.
    /// </summary>
    /// <param name="result"></param>
    public void SetResult(TResult result)
    {
        Model = result;
    }
    
    /// <summary>
    /// Set the error if validation finished unsuccessful. 
    /// </summary>
    /// <param name="error"></param>
    public void SetError(string error)
    {
        Error = error;
    }
}