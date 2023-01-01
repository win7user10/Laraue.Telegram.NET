namespace Laraue.Telegram.NET.Abstractions;

public sealed record RouteExecutionResult(bool IsExecuted, object? ExecutionResult);