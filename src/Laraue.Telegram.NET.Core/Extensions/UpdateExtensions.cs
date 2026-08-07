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
                ?? update.EditedMessage?.GetUser()
                ?? update.CallbackQuery?.GetUser()
                ?? update.InlineQuery?.GetUser();
        }
        
        /// <summary>
        /// Get chat from the telegram <see cref="Update"/>.
        /// </summary>
        public long? TryGetChatId()
        {
            return update.Message?.Chat.Id
                ?? update.EditedMessage?.Chat.Id
                ?? update.CallbackQuery?.Message?.Chat.Id;
        }
        
        /// <summary>
        /// Get user id from the telegram <see cref="Update"/>.
        /// </summary>
        public long GetUserId()
        {
            return update.GetUser().GetId();
        }
    }

    /// <summary>
    /// Get user from the telegram <see cref="Message"/>.
    /// </summary>
    public static User? GetUser(this Message message)
    {
        return message.From;
    }
    
    /// <summary>
    /// Get user from the telegram <see cref="CallbackQuery"/>.
    /// </summary>
    public static User? GetUser(this CallbackQuery callbackQuery)
    {
        return callbackQuery.From;
    }
    
    /// <summary>
    /// Get user from the telegram <see cref="InlineQuery"/>.
    /// </summary>
    public static User? GetUser(this InlineQuery inlineQuery)
    {
        return inlineQuery.From;
    }
    
    /// <summary>
    /// Get user id from the telegram <see cref="User"/>.
    /// </summary>
    public static long GetId(this User? user)
    {
        return user?.Id ?? throw new InvalidOperationException();
    }
}