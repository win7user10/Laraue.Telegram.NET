﻿using System.Collections.Concurrent;
using Laraue.Core.Threading;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Authentication.Extensions;
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
    private readonly IUserGroupProvider _userGroupProvider;

    private static readonly ConcurrentDictionary<long, TKey> UserIdTelegramIdMap = new ();
    private static readonly KeyedSemaphoreSlim<long> Semaphore = new (1);

    public AuthTelegramMiddleware(
        ITelegramMiddleware next,
        IUserService<TKey> userService,
        TelegramRequestContext<TKey> telegramRequestContext,
        ILogger<AuthTelegramMiddleware<TKey>> logger,
        IUserGroupProvider userGroupProvider)
    {
        _next = next;
        _userService = userService;
        _telegramRequestContext = telegramRequestContext;
        _logger = logger;
        _userGroupProvider = userGroupProvider;
    }
    
    /// <inheritdoc />
    public async Task<object?> InvokeAsync(CancellationToken ct)
    {
        var user = _telegramRequestContext.Update.GetUser();
        if (user is null)
        {
            return await _next.InvokeAsync(ct);
        }
        
        using var _ = await Semaphore.WaitAsync(user.Id, ct);

        if (!UserIdTelegramIdMap.TryGetValue(user.Id, out var systemId))
        {
            var result = await _userService.LoginOrRegisterAsync(
                new TelegramData(user.Id, user.Username!));
            
            UserIdTelegramIdMap.TryAdd(user.Id, result.UserId);
            systemId = result.UserId;
        }
        
        _telegramRequestContext.UserId = systemId;

        var userGroups = await _userGroupProvider.GetUserGroupsAsync(user);
        _telegramRequestContext.Groups = userGroups.Select(x => x.Name).ToArray();
        
        _logger.LogInformation(
            "Auth as: telegram id: {TelegramId}, system id: {SystemId}, groups: [{Groups}]",
            user.Id,
            systemId,
            string.Join(',', userGroups));
        
        return await _next.InvokeAsync(ct);
    }
}