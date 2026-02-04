using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Extensions;

/// <summary>
/// Extensions to work with telegram <see cref="Update"/>.
/// </summary>
public static class UpdateExtensions
{
    /// <param name="update"></param>
    extension(Update update)
    {
        /// <summary>
        /// Get user from the telegram <see cref="Update"/>.
        /// </summary>
        public User? GetUser()
        {
            return update.Message?.GetUser()
                ?? update.CallbackQuery?.GetUser();
        }
        
        /// <summary>
        /// Get chat from the telegram <see cref="Update"/>.
        /// </summary>
        public long? TryGetChatId()
        {
            return update.Message?.Chat?.Id
                ?? update.CallbackQuery?.Message?.Chat?.Id;
        }
        
        /// <summary>
        /// Get user id from the telegram <see cref="Update"/>.
        /// </summary>
        /// <returns></returns>
        public long GetUserId()
        {
            return update.GetUser().GetId();
        }
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
}