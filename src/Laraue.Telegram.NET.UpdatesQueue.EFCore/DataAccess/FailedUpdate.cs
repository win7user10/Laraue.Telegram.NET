namespace Laraue.Telegram.NET.UpdatesQueue.EFCore.DataAccess;

public class FailedUpdate
{
    /// <summary>
    /// Telegram update identifier.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Serialized body to update.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Occured error.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Error stacktrace.
    /// </summary>
    public string? StackTrace { get; set; } = string.Empty;
}