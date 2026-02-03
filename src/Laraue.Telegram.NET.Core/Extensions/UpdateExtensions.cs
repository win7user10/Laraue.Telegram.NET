using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Extensions to work with telegram <see cref="Update"/>.
/// </summary>
public static class UpdateExtensions
{
    /// <summary>
    /// Get user from the telegram <see cref="Update"/>.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public static User? GetUser(this Update update)
    {
        return update.Message?.GetUser()
            ?? update.CallbackQuery?.GetUser();
    }
    
    /// <summary>
    /// Get user from the telegram <see cref="Message"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static User? GetUser(this Message message)
    {
        return message.From;
    }
    
    /// <summary>
    /// Get user from the telegram <see cref="CallbackQuery"/>.
    /// </summary>
    /// <param name="callbackQuery"></param>
    /// <returns></returns>
    public static User? GetUser(this CallbackQuery callbackQuery)
    {
        return callbackQuery.From;
    }
    
    /// <summary>
    /// Get user id from the telegram <see cref="User"/>.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static long GetId(this User? user)
    {
        return user?.Id ?? throw new InvalidOperationException();
    }
    
    /// <summary>
    /// Get user id from the telegram <see cref="Update"/>.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public static long GetUserId(this Update update)
    {
        return update.GetUser().GetId();
    }
    
    public static bool TryGetUserId(this Update update, [NotNullWhen(true)] out long? userId)
    {
        userId = update.GetUser()?.Id;
        return userId != null;
    }
}