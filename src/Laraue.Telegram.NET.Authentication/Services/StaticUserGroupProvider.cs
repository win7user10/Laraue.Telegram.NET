using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Static implementation of <see cref="IUserGroupProvider"/> based on const
/// list of groups and users.
/// </summary>
public class StaticUserGroupProvider : IUserGroupProvider
{
    private readonly Dictionary<string, string[]> _userGroups;
    
    /// <summary>
    /// Initializes a new instance of <see cref="StaticUserGroupProvider"/>.
    /// </summary>
    public StaticUserGroupProvider(GroupUsers groupUsers)
    {
        _userGroups = groupUsers
            .SelectMany(x => x.Value, (pair, nickName) => (pair.Key, nickName))
            .GroupBy(x => x.nickName, x => x.Key)
            .ToDictionary(x => x.Key, x => x.ToArray());
    }
    
    /// <inheritdoc />
    public Task<IList<UserGroup>> GetUserGroupsAsync(User user)
    {
        var result = _userGroups.TryGetValue(user.Username ?? string.Empty, out var groups)
            ? groups.Select(x => new UserGroup(x)).ToList()
            : new List<UserGroup>();

        return Task.FromResult((IList<UserGroup>)result);
    }
}

/// <summary>
/// Map between group name and telegram user names that belongs to this group.
/// </summary>
public sealed class GroupUsers : Dictionary<string, string[]>
{}