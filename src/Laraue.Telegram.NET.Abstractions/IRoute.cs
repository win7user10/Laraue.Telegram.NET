namespace Laraue.Telegram.NET.Abstractions;

/// <summary>
/// One of the telegram routes.
/// </summary>
public interface IRoute
{
    /// <summary>
    /// Try execute route if it is suitable for the execution.
    /// </summary>
    /// <param name="requestProvider"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    ValueTask<RouteExecutionResult> TryExecuteAsync(IServiceProvider requestProvider, CancellationToken ct);
}