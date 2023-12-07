using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Default empty implementation if groups functionality is not required.
/// </summary>
public class DefaultUserGroupProvider : IUserGroupProvider
{
    /// <inheritdoc />
    public Task<IList<UserGroup>> GetUserGroupsAsync(User user)
    {
        return Task.FromResult((IList<UserGroup>)new List<UserGroup>());
    }
}