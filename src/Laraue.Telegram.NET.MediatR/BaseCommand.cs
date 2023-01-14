using MediatR;

namespace Laraue.Telegram.NET.MediatR;

/// <summary>
/// This class should be inherited by any telegram request handler.
/// </summary>
/// <typeparam name="TData"></typeparam>
public record BaseCommand<TData> : IRequest
{
    /// <summary>
    /// User identifier in the database.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Data required to execute this command.
    /// </summary>
    public TData? Data { get; init; }
}