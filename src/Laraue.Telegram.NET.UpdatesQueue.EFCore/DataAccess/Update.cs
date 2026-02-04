namespace Laraue.Telegram.NET.UpdatesQueue.EFCore.DataAccess;

public class Update
{
    /// <summary>
    /// Telegram update identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Serialized body to update.
    /// </summary>
    public string Body { get; set; } = string.Empty;
}