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
    private readonly IInterceptorsDbContext<TUserKey> _dbContext;

    /// <inheritdoc />
    public EFCoreInterceptorState(IInterceptorsDbContext<TUserKey> dbContext, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public override Task<string?> GetAsync(
        TUserKey userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.InterceptorState
            .Where(x => x.UserId.Equals(userId))
            .Select(x => x.ActiveInterceptor)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task<TContext?> GetInterceptorContextInternalAsync<TContext>(
        TUserKey userId,
        CancellationToken cancellationToken = default)
        where TContext : class
    {
        var data = await _dbContext.InterceptorState
            .Where(x => x.UserId.Equals(userId))
            .Select(x => x.InterceptorContext)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return data is null ? null : JsonSerializer.Deserialize<TContext>(data);
    }

    /// <inheritdoc />
    protected override Task SetInterceptorAsync<TContext>(
        TUserKey userId,
        string id,
        TContext context,
        CancellationToken cancellationToken = default)
    {
        var model = new InterceptorStateModel<TUserKey>
        {
            ActiveInterceptor = id,
            UserId = userId,
            InterceptorContext = JsonSerializer.Serialize(context)
        };

        _dbContext.InterceptorState.Add(model);

        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override Task ResetAsync(
        TUserKey userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.InterceptorState
            .Where(x => x.UserId.Equals(userId))
            .ExecuteDeleteAsync(cancellationToken);
    }
}