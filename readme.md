# Laraue.Telegram.NET

This library contatins infrastructure code for fast telegram bots building. The library use _Telegram.Bot_ package inside to communicate with Telegram.

## Laraue.Telegram.NET.Core

[![latest version](https://img.shields.io/nuget/v/Laraue.Telegram.NET.Core)](https://www.nuget.org/packages/Laraue.Telegram.NET.Core)

The main idea of this library is to register all possible telegram routes inheriting from the class _TelegramController_.
```csharp
public class SettingsController : TelegramController
{
    private readonly IMediator _mediator;

    public SettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [TelegramMessageRoute("/settings")]
    public Task ShowMenuAsync(TelegramRequestContext requestContext)
    {
        return _mediator.Send(new SendSettingsCommand
        {
            Data = requestContext.Update.Message!,
            UserId = requestContext.UserId,
        });
    }
```
Here _TelegramMessageRoute_ means that if the message with text "/settings" will be send by the telegram user, this method will be executed. Only message update will be handled in this case.
To process callback queries can be used attribute _TelegramCallbackRouteAttribute_, any another request type can be handled by the attribute inherited from _TelegramBaseRouteAttribute_.
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

## Laraue.Telegram.NET.Authentication

[![latest version](https://img.shields.io/nuget/v/Laraue.Telegram.NET.Authentication)](https://www.nuget.org/packages/Laraue.Telegram.NET.Authentication)

This library helps to integrate ASP.NET.Identity with the telegram request context.
To use it the model of user can be defined
```csharp
public class User : TelegramIdentityUser
{}
```
Register Auth functionality in the container
```csharp
services.AddTelegramCore(new TelegramBotClientOptions(builder.Configuration["Telegram:Token"]!))
    .AddTelegramAuthentication<User>()
    .AddEntityFrameworkStores<CianCrawlerDbContext>()
```
**Note** - AddTelegramAuthentication returns _Microsoft.AspNetCore.Identity.IdentityBuilder_, use it to configure identity options.

## Laraue.Telegram.NET.AnswerToQuestion

[![latest version](https://img.shields.io/nuget/v/Laraue.Telegram.NET.AnswerToQuestion)](https://www.nuget.org/packages/Laraue.Telegram.NET.AnswerToQuestion)

This library allow to create an answer functionality. It is suitable for cases when something has been asked from the user and 
next messages should be a response for this question. To use it should be implemented how to store Type that should be used to answer
for question.
```csharp
public class QuestionStateStorage : IQuestionStateStorage
{}
```
Add the functionality to container. AnswerToQuestionFunctionality is depending from Laraue.Telegram.NET.Core and Laraue.Telegram.NET.Authentication packages
and should be registered in this order.
```csharp
services.AddTelegramCore(new TelegramBotClientOptions(builder.Configuration["Telegram:Token"]!))
    .AddAnswerToQuestionFunctionality<QuestionStateStorage>()
    .AddTelegramAuthentication<User>()
```
Then the response awaiter can be implemented
```csharp
public class UpdateAgeResponseAwaiter : BaseAnswerAwaiter<uint>
{
    protected override void Validate(Update update, AnswerResult<TModel> answerResult)
    {
        if (uint.TryParse(update.Message!.Text, out var age))
        {
            answerResult.SetModel(age);
        }
        else
        {
            answerResult.SetError("Age should be a positive number");
        }
    }
    
    protected abstract Task<object?> ExecuteRouteAsync(uint age)
    {
        // Do something with the data.
    }
}
```
There is a short example of awaiter using
```csharp
public class TestController : TelegramController
{
    private readonly IQuestionStateStorage _questionState;
    private readonly ITelegramBotClient _telegramClient;

    public TestController(IQuestionStateStorage questionState, ITelegramBotClient telegramClient)
    {
        _questionStorage = questionState;
        _telegramClient = telegramClient;
    }

    [TelegramMessageRoute("/start")]
    public async Task HandleStart(TelegramRequestContext context)
    {
        await _client.SendTextMessageAsync(
            context.UserId!,
            "What is your age?");
            
        await _questionState.SetAsync<UpdateAgeResponseAwaiter>(context.UserId);
    }
}
```