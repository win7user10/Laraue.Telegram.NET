using System.Reflection;
using System.Text;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Abstractions.Request;
using Laraue.Telegram.NET.Authentication.Attributes;
using Laraue.Telegram.NET.Authentication.Middleware;
using Laraue.Telegram.NET.Authentication.Protectors;
using Laraue.Telegram.NET.Authentication.Services;
using Laraue.Telegram.NET.Core.Extensions;
using Laraue.Telegram.NET.Core.Routing;
using Laraue.Telegram.NET.Core.Routing.Attributes;
using Laraue.Telegram.NET.Interceptors.Extensions;
using Laraue.Telegram.NET.Interceptors.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace Laraue.Telegram.NET.Tests.Controllers;

public class ControllerTests
{
    private readonly TestServer _testServer;
    
    public ControllerTests()
    {
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices((c, s) =>
            {
                s.AddTelegramCore(new TelegramBotClientOptions("a"))
                    .AddScoped<TelegramRequestContext<string>>()
                    .AddScoped<TelegramRequestContext>(
                        sp => sp.GetRequiredService<TelegramRequestContext<string>>())
                    .AddTelegramRequestInterceptors<InMemoryInterceptorState, string>(new[]
                    {
                        Assembly.GetExecutingAssembly(), 
                    }, ServiceLifetime.Singleton)
                    .AddTelegramMiddleware<AuthTelegramMiddleware<string>>()
                    .AddScoped<IUserService<string>, MockedUserService>()
                    .AddScoped<IUserGroupProvider>(sp => new StaticUserGroupProvider(
                        new GroupUsers { ["Protected.View"] = new []{ "JohnLennon" } }))
                    .AddScoped<IControllerProtector, UserShouldBeInGroupProtector<string>>();;
            })
            .Configure(a => a.MapTelegramRequests("/test"));
        
        _testServer = new TestServer(hostBuilder);
    }

    private async Task<string> SendRequestAsync(Update update)
    {
        var resp = await _testServer.CreateClient().PostAsync(
            "test",
            new StringContent(JsonConvert.SerializeObject(update),
                Encoding.UTF8,
                "text/json"));

        resp.EnsureSuccessStatusCode();

        return await resp.Content.ReadAsStringAsync();
    }

    [Fact]
    public async Task MessageRoute_ShouldResponseCorrectlyAsync()
    {
        var result = await SendRequestAsync(new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "Ilya",
                    Username = "user",
                    Id = 123,
                    IsBot = false,
                },
                Text = "message",
                Date = DateTime.UtcNow,
                Chat = new Chat
                {
                    Id = 1,
                }
            },
        });
        
        Assert.Equal("message", result);
    }
    
    [Fact]
    public async Task CallbackRoute_ShouldParsePathParametersCorrectlyAsync()
    {
        var result = await SendRequestAsync(new Update
        {
            CallbackQuery = new CallbackQuery
            {
                From = new User
                {
                    FirstName = "Ilya",
                    Username = "user",
                    Id = 123,
                    IsBot = false,
                },
                Data = "callback/12?name=Alex",
                Id = "123",
                ChatInstance = "123",
            },
        });
        
        Assert.Equal("Alex12Alex", result);
    }
    
    [Fact]
    public async Task MessageRoute_ShouldAwaitResponseOnMessageAsync()
    {
        var sendMessage = new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "Ilya",
                    Username = "user",
                    Id = 123,
                    IsBot = false,
                },
                Text = "awaitResponse",
                Date = DateTime.UtcNow,
                Chat = new Chat
                {
                    Id = 1,
                }
            },
        };
        
        var result = await SendRequestAsync(sendMessage);
        Assert.Equal("awaitResponse", result);
        
        result = await SendRequestAsync(sendMessage);
        Assert.Equal("awaitedHi", result);
        
        result = await SendRequestAsync(sendMessage);
        Assert.Equal("awaitResponse", result);
    }
    
    [Theory]
    [InlineData("JohnLennon", true)]
    [InlineData("AlexFerrari", false)]
    public async Task MessageRoute_ShouldBeProtectedAsync(string userName, bool shouldRouteBeAvailable)
    {
        var result = await SendRequestAsync(new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "Ilya",
                    Username = userName,
                    Id = 123,
                    IsBot = false,
                },
                Text = "protected",
                Date = DateTime.UtcNow,
                Chat = new Chat
                {
                    Id = 1,
                }
            },
        });
        
        Assert.Equal(shouldRouteBeAvailable ? "protected" : string.Empty, result);
    }

    public class TestTelegramController : TelegramController
    {
        private readonly IInterceptorState<string> _responseAwaiter;

        public TestTelegramController(IInterceptorState<string> responseAwaiter)
        {
            _responseAwaiter = responseAwaiter;
        }

        [TelegramMessageRoute("message")]
        public Task<string?> ExecuteMessageAsync(TelegramRequestContext<string> requestContext)
        {
            return Task.FromResult(requestContext.Update.Message!.Text);
        }
        
        [TelegramCallbackRoute("callback/{id}")]
        public Task<string> ExecuteCallbackAsync([FromPath] int id, [FromQuery] string? name, [FromQuery] QueryParameters queryParameters)
        {
            return Task.FromResult($"{name}{id}{queryParameters.Name}");
        }
        
        [TelegramMessageRoute("awaitResponse")]
        public async Task<string?> ExecuteMessageWithResponseAwaiterAsync(TelegramRequestContext<string> requestContext)
        {
            await _responseAwaiter.SetAsync<MessageResponseAwaiter, MessageResponseAwaiterParameters>(
                requestContext.UserId!,
                new MessageResponseAwaiterParameters("Hi"));
            
            return requestContext.Update.Message!.Text;
        }
        
        [RequiresUserGroup("Protected.View")]
        [TelegramMessageRoute("protected")]
        public Task<string?> ExecuteProtectedAsync(TelegramRequestContext<string> requestContext)
        {
            return Task.FromResult(requestContext.Update.Message!.Text);
        }
    }

    public sealed class QueryParameters
    {
        public string Name { get; init; }
    }

    private sealed class MessageResponseAwaiter : BaseRequestInterceptor<string, MessageResponseAwaiterModel, MessageResponseAwaiterParameters>
    {
        public override string Id => "MessageResponseAwaiter";

        protected override Task ValidateAsync(
            TelegramRequestContext<string> requestContext,
            InterceptResult<MessageResponseAwaiterModel> interceptResult,
            MessageResponseAwaiterParameters? interceptorContext)
        {
            interceptResult.SetResult(new MessageResponseAwaiterModel("awaited"));

            return Task.CompletedTask;
        }

        protected override Task<object?> ExecuteRouteAsync(
            TelegramRequestContext<string> requestContext,
            MessageResponseAwaiterModel model,
            MessageResponseAwaiterParameters? interceptorContext)
        {
            return Task.FromResult((object?)(model.Message + interceptorContext?.Message));
        }

        public MessageResponseAwaiter(TelegramRequestContext<string> requestContext, IInterceptorState<string> interceptorState)
            : base(requestContext, interceptorState)
        {
        }
    }
    private sealed record MessageResponseAwaiterParameters(string Message);
    private sealed record MessageResponseAwaiterModel(string Message);

    private class InMemoryInterceptorState : BaseInterceptorState<string>
    {
        private readonly Dictionary<string, (string, object)> _interceptors = new ();

        public InMemoryInterceptorState(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override Task<string?> GetAsync(string userId)
        {
            return Task.FromResult(_interceptors.TryGetValue(userId, out var value)
                ? value.Item1
                : default);
        }

        protected override Task<TContext?> GetInterceptorContextInternalAsync<TContext>(string userId) where TContext : class
        {
            _interceptors.TryGetValue(userId, out var value);

            var e = value.Item2 as TContext;

            return (Task.FromResult(e));
        }

        protected override Task SetInterceptorAsync<TContext>(string userId, string id, TContext context)
        {
            _interceptors[userId] = (id, context);
            
            return Task.CompletedTask;
        }

        public override Task ResetAsync(string userId)
        {
            _interceptors.Remove(userId);
            
            return Task.CompletedTask;
        }
    }

    public sealed class MockedUserService : IUserService<string>
    {
        public Task<LoginResponse<string>> LoginOrRegisterAsync(TelegramData loginData)
        {
            return Task.FromResult(new LoginResponse<string>("123"));
        }
    }
}