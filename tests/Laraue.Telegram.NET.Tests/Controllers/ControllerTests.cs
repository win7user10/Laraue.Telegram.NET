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
                    .AddAnswerToQuestionFunctionality<InMemoryQuestionStateStorage>(ServiceLifetime.Singleton)
                    .AddTelegramMiddleware<AuthTelegramMiddleware>()
                    .AddScoped<IUserService, MockedUserService>();
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
        private readonly IQuestionStateStorage _responseAwaiterStorage;

        public TestTelegramController(IQuestionStateStorage responseAwaiterStorage)
        {
            _responseAwaiterStorage = responseAwaiterStorage;
        }

        [TelegramMessageRoute("message")]
        public Task<string?> ExecuteMessageAsync(TelegramRequestContext requestContext)
        {
            return Task.FromResult(requestContext.Update.Message!.Text);
        }
        
        [TelegramCallbackRoute("callback")]
        public Task<string?> ExecuteCallbackAsync(TelegramRequestContext requestContext)
        {
            return Task.FromResult(requestContext.Update.CallbackQuery!.Data);
        }
        
        [TelegramMessageRoute("awaitResponse")]
        public async Task<string?> ExecuteMessageWithResponseAwaiterAsync(TelegramRequestContext requestContext)
        {
            await _responseAwaiterStorage.SetAsync<MessageResponseAwaiter>(requestContext.UserId!);
            
            return requestContext.Update.Message!.Text;
        }
    }

    private sealed class MessageResponseAwaiter : BaseAnswerAwaiter<MessageResponseAwaiterModel>
    {
        protected override void Validate(TelegramRequestContext requestContext, AnswerResult<MessageResponseAwaiterModel> answerResult)
        {
            answerResult.SetResult(new MessageResponseAwaiterModel("awaited"));
        }

        protected override Task<object?> ExecuteRouteAsync(TelegramRequestContext requestContext, MessageResponseAwaiterModel model)
        {
            return Task.FromResult((object?)model.Message);
        }
    }

    private sealed record MessageResponseAwaiterModel(string Message);

    private class InMemoryQuestionStateStorage : IQuestionStateStorage
    {
        private readonly Dictionary<string, Type> _awaitersMap = new ();

        public Task<Type?> TryGetAsync(string userId)
        {
            _awaitersMap.TryGetValue(userId, out var awaiterType);

            return Task.FromResult(awaiterType);
        }

        public Task SetAsync<TResponseAwaiter>(string userId)
            where TResponseAwaiter : IAnswerAwaiter
        {
            _awaitersMap[userId] = typeof(TResponseAwaiter);
            
            return Task.CompletedTask;
        }

        public Task ResetAsync(string userId)
        {
            _awaitersMap.Remove(userId);
            
            return Task.CompletedTask;
        }
    }

    public sealed class MockedUserService : IUserService
    {
        public Task<LoginResponse> LoginAsync(LoginData loginData)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> RegisterAsync(LoginData loginData)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> LoginOrRegisterAsync(TelegramData loginData)
        {
            return Task.FromResult(new LoginResponse("123"));
        }
    }
}