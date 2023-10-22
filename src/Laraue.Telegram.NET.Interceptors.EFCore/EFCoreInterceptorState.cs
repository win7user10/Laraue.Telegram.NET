using System.Text.Json;
using Laraue.Telegram.NET.Interceptors.Services;
using Microsoft.EntityFrameworkCore;

namespace Laraue.Telegram.NET.Interceptors.EFCore;

/// <summary>
/// EF core storage implementation.
/// </summary>
/// <typeparam name="TUserKey"></typeparam>
public sealed class EFCoreInterceptorState<TUserKey> : BaseInterceptorState<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
    private readonly IInterceptorsContext<TUserKey> _dbContext;

    /// <inheritdoc />
    public EFCoreInterceptorState(IInterceptorsContext<TUserKey> dbContext, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public override async Task<string?> GetAsync(TUserKey userId)
    {
        return await _dbContext.InterceptorState
            .Where(x => x.UserId.Equals(userId))
            .Select(x => x.ActiveInterceptor)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<TContext?> GetInterceptorContextInternalAsync<TContext>(TUserKey userId) where TContext : class
    {
        var data = await _dbContext.InterceptorState
            .Where(x => x.UserId.Equals(userId))
            .Select(x => x.InterceptorContext)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return data is null ? null : JsonSerializer.Deserialize<TContext>(data);
    }

    /// <inheritdoc />
    protected override async Task SetInterceptorAsync<TContext>(TUserKey userId, string id, TContext context)
    {
        var model = new InterceptorStateModel<TUserKey>()
        {
            ActiveInterceptor = id,
            UserId = userId,
            InterceptorContext = JsonSerializer.Serialize(context)
        };

        _dbContext.InterceptorState.Add(model);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task ResetAsync(TUserKey userId)
    {
        await _dbContext.InterceptorState
            .Where(x => x.UserId.Equals(userId))
            .ExecuteDeleteAsync()
            .ConfigureAwait(false);
    }
}