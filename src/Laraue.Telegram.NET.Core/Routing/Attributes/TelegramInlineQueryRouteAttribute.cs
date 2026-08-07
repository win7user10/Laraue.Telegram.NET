using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Telegram.NET.Core.Routing.Attributes;

/// <summary>
/// Route based on the <see cref="Update.InlineQuery"/> property of <see cref="Update"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TelegramInlineQueryRouteAttribute : TelegramBaseRouteWithPathAttribute
{
    /// <summary>
    /// This route will be matched when the message text is matching to the passed pattern.
    /// </summary>
    public TelegramInlineQueryRouteAttribute(string pathPattern)
        : base(UpdateType.InlineQuery, RouteMethod.Get, pathPattern)
    {
    }

    /// <inheritdoc />
    protected override string? GetDataStringFromUpdate(Update update)
    {
        return update.InlineQuery?.Query;
    }
}