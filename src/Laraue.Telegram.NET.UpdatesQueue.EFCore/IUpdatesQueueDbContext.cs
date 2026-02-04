using Laraue.Core.DataAccess.EFCore;
using Laraue.Telegram.NET.UpdatesQueue.EFCore.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Laraue.Telegram.NET.UpdatesQueue.EFCore;

public interface IUpdatesQueueDbContext : IDbContext
{
    /// <summary>
    /// Updates that should be processed.
    /// </summary>
    public DbSet<Update> Updates { get; set; }
    
    /// <summary>
    /// Updates failed to process.
    /// </summary>
    public DbSet<FailedUpdate> FailedUpdates { get; set; }
}