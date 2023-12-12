# Laraue.Telegram.NET

This library contains infrastructure code for fast telegram bots building.
The library use https://github.com/TelegramBots/Telegram.Bot package inside to communicate with Telegram.

## Laraue.Telegram.NET.Core

[![latest version](https://img.shields.io/nuget/v/Laraue.Telegram.NET.Core)](https://www.nuget.org/packages/Laraue.Telegram.NET.Core)

The basic idea of the library is to register all possible telegram routes in classes inherited _TelegramController_.

```csharp
public class MenuController : TelegramController
{
    private readonly IMenuService _service;

    public SettingsController(IMenuService service)
    {
        _service = service;
    }
    
    [TelegramMessageRoute("/start")]
    public Task ShowMenuAsync(TelegramRequestContext requestContext)
    {
        return _service.HandleStartAsync(requestContext.Update.Message!);
    }
```

Here _TelegramMessageRoute_ means that if the message with text "/settings" will be send by the telegram user, this method will be executed. Only message update will be handled in this case.
To process callback queries can be used attribute _TelegramCallbackRouteAttribute_, any another request type can be handled by the attribute inherited from _TelegramBaseRouteAttribute_.

To start using this and other controllers the library should be added with the following way
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTelegramCore(new TelegramBotClientOptions("5118263652:AAHiPDQ8kVcbs2WZWG4Z..."));
var app = builder.Build();
app.MapTelegramRequests("/api/telegram");
app.Run();
```

The token in the TelegramBotClientOptions can be taken via @BotFather bot in the Telegram.
_MapTelegramRequests_ method setup the address which will listen callbacks from the telegram.
This address should be set also on the telegram side by calling the next url:
```
https://api.telegram.org/bot5118263652:AAHiPDQ8kVcbs2WZWG4Z.../setWebhook?url=https://your.host/api/telegram
```

## Laraue.Telegram.NET.Authentication

[![latest version](https://img.shields.io/nuget/v/Laraue.Telegram.NET.Authentication)](https://www.nuget.org/packages/Laraue.Telegram.NET.Authentication)

As soon as each request is usually associated with the specific user, it is convenient to
have information about system user in the request instead of manual finding
user by telegram id when it is required.
This library helps to integrate ASP.NET.Identity with the telegram request context.
To use it the model of user can be defined (example for the user with id of _Guid_ type)
```csharp
public class User : TelegramIdentityUser<Guid>
{}

```
Register Auth functionality in the container. Here is the using of ```User``` model with _Guid_ as identifier.
```csharp
services.AddTelegramCore(new TelegramBotClientOptions(builder.Configuration["Telegram:Token"]!))
    .AddTelegramAuthentication<User, Guid>()
    .AddEntityFrameworkStores<CianCrawlerDbContext>()
    .AddDefaultTokenProviders()
```

After that in each telegram controller method can be retrieved ```TelegramRequestContext<Guid>``` which contains information
about the user made the request.

```csharp
public class UserController : TelegramController
{
    [TelegramMessageRoute("/me")]
    public void PrintMyId(TelegramRequestContext<Guid> requestContext)
    {
        Console.WriteLine(requestContext.UserId); // user id in our storage
        Console.WriteLine(requestContext.Update.GetUserId()); // telegram user id from the update
    }
```

To prevent writing generic type for each request, new context class can be created and added to the container.
```csharp
public sealed class RequestContext : TelegramRequestContext<Guid>
{}
```

```csharp
services.AddTelegramAuthentication<User, Guid, RequestContext>()
```

Now the class ```RequestContext``` can be injected without defining request generic type in the each request.

```csharp
public class UserController : TelegramController
{
    [TelegramMessageRoute("/also-me")]
    public void PrintMyId(RequestContext requestContext)
    {
        Console.WriteLine(requestContext.UserId);
    }
```

**Note** - ```AddTelegramAuthentication()``` returns ```Microsoft.AspNetCore.Identity.IdentityBuilder```,
use it to configure identity options.

The logic of retrieving user id field is next: from the received telegram message telegram identifier of the user that send the message takes.
Then this identifier maps to the identifier in the database. Then this identifier sets to the request context.

### Middleware
The package has the opportunity to extend request pipeline by adding custom middlewares. The example is the next
```csharp
public class LogExceptionsMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly TelegramRequestContext _telegramRequestContext;

    public LogExceptionsMiddleware(
        ITelegramMiddleware next,
        TelegramRequestContext telegramRequestContext)
    {
        _next = next;
        _telegramRequestContext = telegramRequestContext;
    }
    
    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        try
        {
            return await _next.InvokeAsync(ct);
        }
        catch (BadTelegramRequestException ex)
        {
            _logger.LogError(ex, "Error occured");
        }

        return null;
    }
}
```
Adding middleware to the request pipeline
```csharp
services.AddTelegramMiddleware<LogExceptionsMiddleware>();
```
**Note** - middlewares executes in the order they were added.


## Laraue.Telegram.NET.Interceptors

[![latest version](https://img.shields.io/nuget/v/Laraue.Telegram.NET.Interceptors)](https://www.nuget.org/packages/Laraue.Telegram.NET.Interceptors)

The main case of the library is sometimes something from the user should be asked,
and next his answer should be considered as answer to this question.
This library allow to create such functionality. To use it should be implemented how to store Type that should be used to answer
for question.
```csharp
public class InterceptorState : BaseInterceptorState
{
    protected override Task<string?> TryGetStringIdentifierFromStorageAsync(string userId)
    {
        throw new NotImplementedException();
    }

    protected override Task SetStringIdentifierToStorageAsync(string userId, string id)
    {
        throw new NotImplementedException();
    }

    public override Task ResetAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
```
Add the functionality to container. Interceptors are depending on _Laraue.Telegram.NET.Core_ 
and _Laraue.Telegram.NET.Authentication_ packages and should be registered in such order.
```csharp
services.AddTelegramCore(new TelegramBotClientOptions(builder.Configuration["Telegram:Token"]!))
    .AddTelegramRequestEfCoreInterceptors<InterceptorState>()
    .AddTelegramAuthentication<User, Guid>()
```
**Note** - there is already implemented IInterceptorState with storing state in the DB via EFCore in the package
_Laraue.Telegram.NET.Interceptors.EFCore_.
```csharp
services.AddTelegramRequestEfCoreInterceptors<Guid, MyDbContext>(assemblies})
```

After interceptors state storage is setup, the interceptor can be implemented
```csharp
public class UpdateAgeInterceptor : BaseRequestInterceptor<Guid, uint, UpdateAgeContext>
{
    private readonly IUserRepository _repository;
    
    public UpdateAgeResponseInterceptor(IUserRepository repository)
    {
        _repository = repository;
    }
    
    protected override Task ValidateAsync(
        TelegramRequestContext<Guid> requestContext,
        InterceptResult<uint> interceptResult,
        UpdateAgeContext interceptorContext)
    {
        if (uint.TryParse(update.Message!.Text, out var age))
        {
            interceptResult.SetModel(age);
        }
        else
        {
            interceptResult.SetError("Age should be a positive number");
        }
        
        return Task.CompletedTask;
    }
    
    protected abstract Task ExecuteRouteAsync(
        TelegramRequestContext<Guid> requestContext,
        uint model,
        UpdateAgeContext interceptorContext)
    {
        return _repository.UpdateUserAgeAsync(interceptorContext.UserId, model);
    }
}

```
There is a short example of interceptor using
```csharp
public class TestController : TelegramController
{
    private readonly IInterceptorState _questionState;
    private readonly ITelegramBotClient _telegramClient;

    public TestController(IInterceptorState questionState, ITelegramBotClient telegramClient)
    {
        _questionStorage = questionState;
        _telegramClient = telegramClient;
    }

    [TelegramMessageRoute("/start")]
    public async Task HandleStart(TelegramRequestContext context)
    {
        await _client.SendTextMessageAsync(
            context.Update.GetUserId(),
            "What is your age?");
            
        await _questionState.SetAsync<UpdateAgeInterceptor>(context.UserId, new UpdateAgeInterceptor
        {
            UserId = context.UserId
        });
    }
}
```