using MediatR;

namespace Laraue.Telegram.NET.MediatR;

/// <summary>
/// This class should be inherited by any telegram request handler.
/// </summary>
/// <typeparam name="TUserKey"></typeparam>
/// <typeparam name="TData"></typeparam>
public record BaseCommand<TUserKey, TData> : IRequest where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// User identifier in the database.
    /// </summary>
    public TUserKey? UserId { get; init; }

    /// <summary>
    /// Data required to execute this command.
    /// </summary>
    public TData? Data { get; init; }
}