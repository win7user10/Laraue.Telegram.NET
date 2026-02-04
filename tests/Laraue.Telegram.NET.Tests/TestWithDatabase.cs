using Laraue.Telegram.NET.UpdatesQueue.EFCore;
using Laraue.Telegram.NET.UpdatesQueue.EFCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Laraue.Telegram.NET.Tests;

[Collection("DatabaseTests")]
public abstract class TestWithDatabase
{
    protected TestWithDatabase()
    {
        var context = GetDbContext();
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
    
    protected TestDbContext GetDbContext()
    {
        return new TestDbContext(new DbContextOptionsBuilder()
            .UseSqlite("Data Source=test.db;")
            .Options);
    }

    protected IServiceCollection ServiceCollection
    {
        get
        {
            var sc = new ServiceCollection()
                .AddSingleton<TestDbContext>(_ => GetDbContext())
                .AddSingleton<IUpdatesQueueDbContext>(sp => sp.GetRequiredService<TestDbContext>());

            return sc;
        }
    }
}

public class TestDbContext : DbContext, IUpdatesQueueDbContext
{
    public TestDbContext(DbContextOptions options) : base(options)
    {}
    
    public DbSet<Update> Updates { get; set; }
    public DbSet<FailedUpdate> FailedUpdates { get; set; }
}