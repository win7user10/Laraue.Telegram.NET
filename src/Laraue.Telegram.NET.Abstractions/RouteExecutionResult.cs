namespace Laraue.Telegram.NET.Abstractions;

/// <summary>
/// Contains info about route execution.
/// </summary>
/// <param name="IsExecuted"></param>
/// <param name="ExecutionResult"></param>
public sealed record RouteExecutionResult(bool IsExecuted, object? ExecutionResult);