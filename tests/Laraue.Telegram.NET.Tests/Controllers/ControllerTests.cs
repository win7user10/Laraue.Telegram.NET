using System.Text;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.AnswerToQuestion.Extensions;
using Laraue.Telegram.NET.AnswerToQuestion.Services;
using Laraue.Telegram.NET.Authentication.Middleware;
using Laraue.Telegram.NET.Authentication.Services;
using Laraue.Telegram.NET.Core.Extensions;
using Laraue.Telegram.NET.Core.Routing;
using Laraue.Telegram.NET.Core.Routing.Attributes;
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
                    .AddAnswerToQuestionFunctionality<InMemoryQuestionStateStorage, string>(ServiceLifetime.Singleton)
                    .AddTelegramMiddleware<AuthTelegramMiddleware<string>>()
                    .AddScoped<IUserService<string>, MockedUserService>();
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
    public async Task CallbackRoute_ShouldResponseCorrectlyAsync()
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
                Data = "callback",
                Id = "123",
                ChatInstance = "123",
            },
        });
        
        Assert.Equal("callback", result);
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
        Assert.Equal("awaited", result);
        
        result = await SendRequestAsync(sendMessage);
        Assert.Equal("awaitResponse", result);
    }

    public class TestTelegramController : TelegramController
    {
        private readonly IQuestionStateStorage<string> _responseAwaiterStorage;

        public TestTelegramController(IQuestionStateStorage<string> responseAwaiterStorage)
        {
            _responseAwaiterStorage = responseAwaiterStorage;
        }

        [TelegramMessageRoute("message")]
        public Task<string?> ExecuteMessageAsync(TelegramRequestContext<string> requestContext)
        {
            return Task.FromResult(requestContext.Update.Message!.Text);
        }
        
        [TelegramCallbackRoute("callback")]
        public Task<string?> ExecuteCallbackAsync(TelegramRequestContext<string> requestContext)
        {
            return Task.FromResult(requestContext.Update.CallbackQuery!.Data);
        }
        
        [TelegramMessageRoute("awaitResponse")]
        public async Task<string?> ExecuteMessageWithResponseAwaiterAsync(TelegramRequestContext<string> requestContext)
        {
            await _responseAwaiterStorage.SetAsync<MessageResponseAwaiter>(requestContext.UserId!);
            
            return requestContext.Update.Message!.Text;
        }
    }

    private sealed class MessageResponseAwaiter : BaseAnswerAwaiter<string, MessageResponseAwaiterModel>
    {
        public override string Id => "MessageResponseAwaiter";
        
        protected override void Validate(TelegramRequestContext<string> requestContext, AnswerResult<MessageResponseAwaiterModel> answerResult)
        {
            answerResult.SetResult(new MessageResponseAwaiterModel("awaited"));
        }

        protected override Task<object?> ExecuteRouteAsync(TelegramRequestContext<string> requestContext, MessageResponseAwaiterModel model)
        {
            return Task.FromResult((object?)model.Message);
        }
    }

    private sealed record MessageResponseAwaiterModel(string Message);

    private class InMemoryQuestionStateStorage : BaseQuestionStateStorage<string>
    {
        private readonly Dictionary<string, string> _awaiterStorage = new ();

        public InMemoryQuestionStateStorage(IEnumerable<IAnswerAwaiter> awaiters, IServiceProvider serviceProvider)
            : base(awaiters, serviceProvider)
        {
        }

        protected override Task<string?> TryGetStringIdentifierFromStorageAsync(string userId)
        {
            _awaiterStorage.TryGetValue(userId, out var value);

            return Task.FromResult(value);
        }

        protected override Task SetStringIdentifierToStorageAsync(string userId, string id)
        {
            _awaiterStorage[userId] = id;
            
            return Task.CompletedTask;
        }

        public override Task ResetAsync(string userId)
        {
            _awaiterStorage.Remove(userId);
            
            return Task.CompletedTask;
        }
    }

    public sealed class MockedUserService : IUserService<string>
    {
        public Task<LoginResponse<string>> LoginAsync(LoginData loginData)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse<string>> RegisterAsync(LoginData loginData)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse<string>> LoginOrRegisterAsync(TelegramData loginData)
        {
            return Task.FromResult(new LoginResponse<string>("123"));
        }
    }
}