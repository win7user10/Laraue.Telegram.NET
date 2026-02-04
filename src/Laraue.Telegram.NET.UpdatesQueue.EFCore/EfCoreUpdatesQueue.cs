using System.Text.Json;
using Laraue.Telegram.NET.Core.Services;
using Laraue.Telegram.NET.UpdatesQueue.EFCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Update = Telegram.Bot.Types.Update;

namespace Laraue.Telegram.NET.UpdatesQueue.EFCore;

public class EfCoreUpdatesQueue(
    IUpdatesQueueDbContext dbContext)
    : IUpdatesQueue
{
    public async Task AddAsync(IEnumerable<Update> updates, CancellationToken cancellationToken = default)
    {
        var updatesArray = updates.ToArray();
        if (updatesArray.Length > 0)
        {
            var entities = updatesArray
                .Select(u => new DataAccess.Update
                {
                    Id = u.Id,
                    Body = JsonSerializer.Serialize(u, JsonBotAPI.Options),
                });
        
            dbContext.Updates.AddRange(entities);
            
            await dbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public Task SetProcessedAsync(Update update, CancellationToken cancellationToken = default)
    {
        return dbContext.Updates
            .Where(u => u.Id == update.Id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SetFailedAsync(
        Update update,
        string error,
        string? stackTrace,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        await 
            SetProcessedAsync(update, cancellationToken)
            .ConfigureAwait(false);

        var failedUpdate = new FailedUpdate
        {
            Id = update.Id,
            Body = JsonSerializer.Serialize(update, JsonBotAPI.Options),
            Error = error,
            StackTrace = stackTrace,
        };

        dbContext.FailedUpdates.Add(failedUpdate);
        
        await dbContext
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<Update[]> GetAsync(int count, CancellationToken cancellationToken = default)
    {
        var updates = await dbContext.Updates
            .OrderBy(u => u.Id)
            .Take(count)
            .ToArrayAsync(cancellationToken);

        return updates
            .Select(u => JsonSerializer.Deserialize<Update>(u.Body, JsonBotAPI.Options)!)
            .ToArray();
    }

    public Task<int> GetLastUpdateIdAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Updates
            .Select(u => u.Id)
            .DefaultIfEmpty()
            .MaxAsync(cancellationToken);
    }
}