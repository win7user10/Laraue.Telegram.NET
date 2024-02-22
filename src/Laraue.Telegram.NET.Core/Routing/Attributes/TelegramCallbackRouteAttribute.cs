using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Telegram.NET.Core.Routing.Attributes;

/// <summary>
/// Route based on the <see cref="Update.CallbackQuery"/> property of <see cref="Update"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TelegramCallbackRouteAttribute : TelegramBaseRouteWithPathAttribute
{
    /// <summary>
    /// This route will be matched when callback query data is matching to the passed pattern.
    /// </summary>
    public TelegramCallbackRouteAttribute(string pathPattern, RouteMethod routeMethod = RouteMethod.Get)
        : base(UpdateType.CallbackQuery, routeMethod, pathPattern)
    {
    }
    
    /// <inheritdoc />
    protected override string? GetDataStringFromUpdate(Update update)
    {
        return update.CallbackQuery?.Data;
    }
}