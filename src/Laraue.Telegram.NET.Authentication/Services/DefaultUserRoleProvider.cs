using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Default empty implementation if groups functionality is not required.
/// </summary>
public class DefaultUserRoleProvider : IUserRoleProvider
{
    /// <inheritdoc />
    public Task<IList<UserRole>> GetUserGroupsAsync(User user)
    {
        return Task.FromResult((IList<UserRole>)new List<UserRole>());
    }
}