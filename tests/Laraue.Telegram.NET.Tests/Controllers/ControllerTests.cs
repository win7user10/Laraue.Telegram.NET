using System.Globalization;
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
using Laraue.Telegram.NET.Localization;
using Laraue.Telegram.NET.Localization.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace Laraue.Telegram.NET.Tests.Controllers;

public class ControllerTests
{
    private readonly TestServer _testServer;
    private readonly Mock<ITestChecker> _testChecker = new();
    
    public ControllerTests()
    {
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices((c, s) =>
            {
                s.AddTelegramCore(new TelegramBotClientOptions("a"))
                    .AddScoped<TelegramRequestContext<string>>()
                    .AddScoped<TelegramRequestContext>(
                        sp => sp.GetRequiredService<TelegramRequestContext<string>>())
                    .AddTelegramRequestLocalization()
                    .Configure<TelegramRequestLocalizationOptions>(opt =>
                    {
                        opt.DefaultLanguage = "en";
                        opt.AvailableLanguages = ["en", "ru"];
                    })
                    .AddTelegramRequestInterceptors<InMemoryInterceptorState, string>(new[]
                    {
                        Assembly.GetExecutingAssembly(),
                    }, ServiceLifetime.Singleton)
                    .AddTelegramMiddleware<AuthTelegramMiddleware<string>>()
                    .AddScoped<IUserService<string>, MockedUserService>()
                    .AddScoped<IUserRoleProvider>(_ => new StaticUserRoleProvider(
                        Options.Create(new RoleUsers { ["Protected.View"] = new[] { "JohnLennon" } })))
                    .AddScoped<IControllerProtector, UserShouldBeInGroupProtector<string>>()
                    .AddSingleton(_testChecker.Object);
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
        
        _testChecker.Verify(x => x.Call("message"));
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
                Data = "callback/12?name=\"Alex\"",
                Id = "123",
                ChatInstance = "123",
            },
        });
        
        _testChecker.Verify(x => x.Call("AlexAlex1212Alex"));
    }
    
    [Fact]
    public async Task MessageRoute_ShouldWorkWithInterceptorsAsync()
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
        _testChecker.Verify(x => x.Call("awaitResponse"));
        
        await SendRequestAsync(sendMessage);
        _testChecker.Verify(x => x.Call("intercepted"));
        
        result = await SendRequestAsync(sendMessage);
        _testChecker.Verify(x => x.Call("awaitResponse"));
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

        if (shouldRouteBeAvailable)
        {
            _testChecker.Verify(x => x.Call("protected"));
        }
        else
        {
            _testChecker.VerifyNoOtherCalls();
        }
    }
    
    [Fact]
    public async Task MessageRoute_ShouldBeLocalizedAsync()
    {
        await SendRequestAsync(new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "Ilya",
                    Username = "user",
                    Id = 123,
                    IsBot = false,
                    LanguageCode = "ru"
                },
                Text = "localizedRoute",
                Date = DateTime.UtcNow,
                Chat = new Chat
                {
                    Id = 1,
                }
            },
        });
        
        _testChecker.Verify(x => x.Call("Locale: ru ru"));
    }

    public class TestTelegramController : TelegramController
    {
        private readonly IInterceptorState<string> _responseAwaiter;
        private readonly ITestChecker _testChecker;

        public TestTelegramController(IInterceptorState<string> responseAwaiter, ITestChecker testChecker)
        {
            _responseAwaiter = responseAwaiter;
            _testChecker = testChecker;
        }

        [TelegramMessageRoute("message")]
        public Task ExecuteMessageAsync(TelegramRequestContext<string> requestContext)
        {
            _testChecker.Call(requestContext.Update.Message!.Text);
            
            return Task.CompletedTask;
        }
        
        [TelegramCallbackRoute("callback/{id}")]
        public Task ExecuteCallbackAsync(
            [FromPath] int id,
            [FromPath("id")] int alsoId,
            [FromQuery] string? name,
            [FromQuery("name")] string? alsoName,
            [FromQuery] QueryParameters queryParameters)
        {
            _testChecker.Call($"{name}{alsoName}{id}{alsoId}{queryParameters.Name}");
            
            return Task.CompletedTask;
        }
        
        [TelegramMessageRoute("awaitResponse")]
        public async Task ExecuteMessageWithResponseAwaiterAsync(TelegramRequestContext<string> requestContext)
        {
            await _responseAwaiter.SetAsync<MessageResponseAwaiter, MessageResponseAwaiterParameters>(
                requestContext.UserId!,
                new MessageResponseAwaiterParameters("Hi"));
            
            _testChecker.Call(requestContext.Update.Message!.Text);
        }
        
        [RequiresUserRole("Protected.View")]
        [TelegramMessageRoute("protected")]
        public Task ExecuteProtectedAsync(TelegramRequestContext<string> requestContext)
        {
            _testChecker.Call(requestContext.Update.Message!.Text);
            
            return Task.CompletedTask;
        }
        
        [TelegramMessageRoute("localizedRoute")]
        public void ExecuteLocalizedRoute(TelegramRequestContext<string> requestContext)
        {
            _testChecker.Call($"Locale: {CultureInfo.CurrentCulture} {CultureInfo.CurrentUICulture}");
        }
    }

    public sealed class QueryParameters
    {
        public string Name { get; init; }
    }

    private sealed class MessageResponseAwaiter : BaseRequestInterceptor<string, MessageResponseAwaiterModel, MessageResponseAwaiterParameters>
    {
        private readonly ITestChecker _testChecker;
        
        public override string Id => "MessageResponseAwaiter";

        protected override Task ValidateAsync(
            TelegramRequestContext<string> requestContext,
            InterceptResult<MessageResponseAwaiterModel> interceptResult,
            MessageResponseAwaiterParameters? interceptorContext)
        {
            interceptResult.SetResult(new MessageResponseAwaiterModel("awaited"));

            return Task.CompletedTask;
        }

        protected override Task<ExecutionState> ExecuteRouteAsync(
            TelegramRequestContext<string> requestContext,
            MessageResponseAwaiterModel model,
            MessageResponseAwaiterParameters? interceptorContext)
        {
            _testChecker.Call("intercepted");
            
            return Task.FromResult(ExecutionState.FullyExecuted);
        }

        public MessageResponseAwaiter(TelegramRequestContext<string> requestContext, IInterceptorState<string> interceptorState, ITestChecker testChecker)
            : base(requestContext, interceptorState)
        {
            _testChecker = testChecker;
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
    
    public interface ITestChecker
    {
        void Call(string? testValue);
    }
}