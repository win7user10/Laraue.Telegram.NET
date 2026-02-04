using Laraue.Telegram.NET.UpdatesQueue.EFCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Xunit;

namespace Laraue.Telegram.NET.Tests.UpdatesQueue;

public class EfCoreUpdatesQueueTests : TestWithDatabase
{
    [Fact]
    public async Task DoubleAdd_ShouldSkipDuplicates_Always()
    {
        var updatesQueue = GetService();
        
        await updatesQueue.AddAsync(
            [new Update { Id = 1, }],
            TestContext.Current.CancellationToken);
        
        await updatesQueue.AddAsync(
            [new Update { Id = 1, }],
            TestContext.Current.CancellationToken);

        var updates = await updatesQueue.GetAsync(2, TestContext.Current.CancellationToken);
        Assert.Single(updates);
    }

    [Fact]
    public async Task SetProcessed_ShouldRemoveEntryFromTheQueue_Always()
    {
        var updatesQueue = GetService();
        
        await updatesQueue.AddAsync(
            [new Update { Id = 1, }],
            TestContext.Current.CancellationToken);

        var updates = await updatesQueue.GetAsync(1, TestContext.Current.CancellationToken);
        var update = Assert.Single(updates);

        await updatesQueue.SetProcessedAsync(update, TestContext.Current.CancellationToken);
        updates = await updatesQueue.GetAsync(1, TestContext.Current.CancellationToken);
        Assert.Empty(updates);
    }

    private EfCoreUpdatesQueue GetService()
    {
        var provider = ServiceCollection
            .AddSingleton<EfCoreUpdatesQueue>()
            .BuildServiceProvider();
        
        return provider.GetRequiredService<EfCoreUpdatesQueue>();
    }
}