using System.ComponentModel.DataAnnotations;

namespace Laraue.Telegram.NET.Interceptors.EFCore;

/// <summary>
/// Entity for interceptor states.
/// </summary>
/// <typeparam name="TUserKey"></typeparam>
public class InterceptorStateModel<TUserKey> where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// User id reference.
    /// </summary>
    [Key]
    public TUserKey UserId { get; set; }

    /// <summary>
    /// Currently active interceptor.
    /// </summary>
    public string? ActiveInterceptor { get; set; }

    /// <summary>
    /// Serialized json data for the interceptor.
    /// </summary>
    public string? InterceptorContext { get; set; }
}