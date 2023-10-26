using Laraue.Core.DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Laraue.Telegram.NET.Interceptors.EFCore;

/// <summary>
/// Database context stores the state of users interceptor state.
/// </summary>
/// <typeparam name="TUserKey"></typeparam>
public interface IInterceptorsDbContext<TUserKey> : IDbContext where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// The interceptors states table.
    /// </summary>
    public DbSet<InterceptorStateModel<TUserKey>> InterceptorState { get; set; }
}