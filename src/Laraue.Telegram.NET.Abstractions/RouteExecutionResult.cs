namespace Laraue.Telegram.NET.Abstractions;

/// <summary>
/// Contains info about route execution.
/// </summary>
/// <param name="IsExecuted"></param>
public sealed record RouteExecutionResult(bool IsExecuted);