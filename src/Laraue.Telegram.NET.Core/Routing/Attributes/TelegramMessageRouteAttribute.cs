﻿using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Telegram.NET.Core.Routing.Attributes;

/// <summary>
/// Route based on the <see cref="Update.Message"/> property of <see cref="Update"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TelegramMessageRouteAttribute : TelegramBaseRouteWithPathAttribute
{
    /// <summary>
    /// This route will be matched when the message text is matching to the passed pattern.
    /// </summary>
    /// <param name="pathPattern"></param>
    public TelegramMessageRouteAttribute(string pathPattern)
        : base(UpdateType.Message, pathPattern)
    {
    }

    /// <inheritdoc />
    protected override string? GetPathFromUpdate(Update update)
    {
        return update.Message?.Text;
    }
}