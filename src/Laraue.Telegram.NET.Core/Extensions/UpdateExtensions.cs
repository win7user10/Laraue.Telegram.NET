using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Extensions;

public static class UpdateExtensions
{
    /// <summary>
    /// Get user from the telegram <see cref="Update"/>.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public static User? GetUser(this Update update)
    {
        return update.Message?.From
            ?? update.CallbackQuery?.From;
    }
    
    /// <summary>
    /// Get user id from the telegram <see cref="Update"/>.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public static long GetUserId(this Update update)
    {
        return update.GetUser()!.Id;
    }
}