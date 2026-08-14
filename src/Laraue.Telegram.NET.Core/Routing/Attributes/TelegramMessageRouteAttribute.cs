using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Telegram.NET.Core.Routing.Attributes;

/// <summary>
/// Route based on the <see cref="Update.Message"/> property of <see cref="Update"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TelegramMessageRouteAttribute : TelegramBaseRouteWithPathAttribute
{
    private readonly ChatType[] _allowedChatTypes;
    private static readonly ChatType[] AllChatTypes = Enum.GetValues<ChatType>();

    /// <summary>
    /// This route will be matched when the message text is matching to the passed pattern.
    /// </summary>
    public TelegramMessageRouteAttribute(string pathPattern)
        : this(pathPattern, AllChatTypes)
    {
    }
    
    /// <summary>
    /// This route will be matched when the message text is matching to the passed pattern and the chat is matching one of passed values.
    /// </summary>
    public TelegramMessageRouteAttribute(string pathPattern, params ChatType[] allowedChatTypes)
        : base(UpdateType.Message, RouteMethod.Get, pathPattern)
    {
        _allowedChatTypes = allowedChatTypes;
    }

    /// <inheritdoc />
    protected override string? GetDataStringFromUpdate(Update update)
    {
        return update.Message?.Text;
    }

    public override bool TryMatch(Update update, [NotNullWhen(true)] out RequestParameters? requestParameters)
    {
        if (_allowedChatTypes.Contains(update.Message!.Chat.Type))
            return base.TryMatch(update, out requestParameters);
        
        requestParameters = null;
        return false;
    }
}