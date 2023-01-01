using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Telegram.NET.Core.Routing.Attributes;

/// <summary>
/// Route based on the <see cref="Update.CallbackQuery"/> property of <see cref="Update"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TelegramCallbackRouteAttribute : TelegramBaseRouteWithPathAttribute
{
    public TelegramCallbackRouteAttribute(string pathPattern)
        : base(UpdateType.CallbackQuery, pathPattern)
    {
    }

    protected override string? GetPathFromUpdate(Update update)
    {
        return update.CallbackQuery?.Data;
    }
}