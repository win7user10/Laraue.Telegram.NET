using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Authentication.Services;
using Laraue.Telegram.NET.Testing.Mocks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Testing;

/// <summary>
/// The base test server class for the Telegram Host.
/// Telegram communication is mocked as default. All telegram calls can be taken via
/// <c>Requests()</c>. To make a call to the Test Server use <c>SendUpdateAsync()</c>.
/// </summary>
public abstract class TelegramTestHost : IDisposable
{
    protected readonly TestServer TestServer;
    private readonly List<IRequest> _requests = [];

    public TelegramTestHost(IServiceCollection serviceCollection)
    {
        var botClient = new TelegramBotClientMock();
        
        var services = serviceCollection
            .AddSingleton<ITelegramBotClient>(botClient)
            .BuildServiceProvider();

        botClient.OnMakingApiRequest += (_, args, _) =>
        {
            _requests.Add(args.Request);
            return ValueTask.CompletedTask;
        };
        
        TestServer = new TestServer(services);
        
        BeforeFirstRequest();
    }

    protected abstract void BeforeFirstRequest();
    
    public async Task SendUpdateAsync(Update update)
    {
        using var requestScope = TestServer.Services.CreateScope();

        var router = requestScope.ServiceProvider.GetRequiredService<ITelegramRouter>();
        
        await router.RouteAsync(update);
    }

    public TelegramRequests Requests()
    {
        return new TelegramRequests(_requests);
    }
    
    public IServiceScope CreateScope()
    {
        return TestServer.Services.CreateScope();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            TestServer.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// The <see cref="TelegramTestHost"/> with mocked user services.
/// </summary>
/// <typeparam name="TUserKey">User primary key type.</typeparam>
public abstract class TelegramTestHost<TUserKey> : TelegramTestHost
    where TUserKey : IEquatable<TUserKey>
{
    protected TelegramTestHost(IServiceCollection serviceCollection)
        : base(serviceCollection
            .AddSingleton<IUserIdByTelegramIdCache<TUserKey>, MockUserIdByTelegramIdCache<TUserKey>>())
    {
    }
}