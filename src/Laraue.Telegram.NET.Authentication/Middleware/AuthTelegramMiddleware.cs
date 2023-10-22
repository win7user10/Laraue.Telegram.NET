using System.Collections.Concurrent;
using Laraue.Core.Threading;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Authentication.Services;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Laraue.Telegram.NET.Authentication.Middleware;

public class AuthTelegramMiddleware<TKey> : ITelegramMiddleware
    where TKey : IEquatable<TKey>
{
    private readonly ITelegramMiddleware _next;
    private readonly IUserService<TKey> _userService;
    private readonly TelegramRequestContext<TKey> _telegramRequestContext;
    private readonly ILogger<AuthTelegramMiddleware<TKey>> _logger;

    private static readonly ConcurrentDictionary<long, TKey> UserIdTelegramIdMap = new ();
    private static readonly KeyedSemaphoreSlim<long> Semaphore = new (1);

    public AuthTelegramMiddleware(
        ITelegramMiddleware next,
        IUserService<TKey> userService,
        TelegramRequestContext<TKey> telegramRequestContext,
        ILogger<AuthTelegramMiddleware<TKey>> logger)
    {
        _next = next;
        _userService = userService;
        _telegramRequestContext = telegramRequestContext;
        _logger = logger;
    }
    
    public async Task<object?> InvokeAsync(CancellationToken ct)
    {
        var from = _telegramRequestContext.Update.GetUser()!;
        
        using var _ = await Semaphore.WaitAsync(from.Id, ct);

        if (!UserIdTelegramIdMap.TryGetValue(from.Id, out var systemId))
        {
            var result = await _userService.LoginOrRegisterAsync(
                new TelegramData(from.Id, from.Username!));
            
            UserIdTelegramIdMap.TryAdd(from.Id, result.UserId);
            systemId = result.UserId;
        }
        
        _telegramRequestContext.UserId = systemId;
        
        _logger.LogInformation("Auth as: telegram id: {TelegramId}, system id: {SystemId}", from.Id, systemId);
        return await _next.InvokeAsync(ct);
    }
}